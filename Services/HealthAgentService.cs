using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MediAgent.Api.Agents;
using MediAgent.Api.Data;
using MediAgent.Api.Entities;
using MediAgent.Api.Infrastructure.WebSearch;
using MediAgent.Api.Models.Requests;
using MediAgent.Api.Models.Responses;

namespace MediAgent.Api.Services;

public class HealthAgentService : IHealthAgentService
{
    private readonly MediAgentKernel _agentKernel;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebSearchService _webSearch;
    private readonly ILogger<HealthAgentService> _logger;

    public HealthAgentService(
        MediAgentKernel agentKernel,
        ApplicationDbContext dbContext,
        IWebSearchService webSearch,
        ILogger<HealthAgentService> logger)
    {
        _agentKernel = agentKernel;
        _dbContext = dbContext;
        _webSearch = webSearch;
        _logger = logger;
    }

    public async Task<SymptomAnalysisResponse> AnalyzeSymptomsAsync(string userId, SymptomAnalysisRequest request)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var conversation = await GetOrCreateConversationAsync(userId, request);

        var userMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "User",
            Content = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Messages.Add(userMessage);
        //when user want to analyze the symptoms
        //the server was response the analysis result
        //one conversation have a many message

        var agentResponse = await _agentKernel.AnalyzeSymptomsAsync(request.Description);

        var parsedResponse = ParseAgentResponse(agentResponse.Content);

        var resources = new List<MedicalResource>();
        if (parsedResponse.PossibleConditions?.Any() == true)
        {
            var searchResults = await _webSearch.SearchResourcesAsync(
                parsedResponse.PossibleConditions.First().Name);
            resources = searchResults.Select(r => new MedicalResource
            {
                Id = Guid.NewGuid(),
                Title = r.Title,
                Url = r.Url,
                Source = r.Source,
                Description = r.Description,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }
        //medical resource message with link was store in database when agent was responsed

        var assistantMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = "Assistant",
            Content = agentResponse.Content,
            PossibleConditions = JsonSerializer.Serialize(parsedResponse.PossibleConditions),
            HealthTips = JsonSerializer.Serialize(parsedResponse.RecommendedActions),
            ResourceLinks = JsonSerializer.Serialize(resources),
            SafetyFlags = JsonSerializer.Serialize(new[] { parsedResponse.UrgencyLevel }),//urgency level
            ModelUsed = agentResponse.ModelUsed,
            ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds,//processing time in milliseconds
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Messages.Add(assistantMessage);
        //agent response was store in database when agent was responsed
        //one conversation have a many message

        foreach (var resource in resources)
        {
            resource.MessageId = assistantMessage.Id;
            _dbContext.MedicalResources.Add(resource);
            //list of medical resource link message was stored in database when agent was responsed
        }

        conversation.UrgencyLevel = parsedResponse.UrgencyLevel ?? "Low";//urgency level default is Low
        conversation.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new SymptomAnalysisResponse
        {
            ConversationId = conversation.Id,
            Analysis = parsedResponse,
            HealthTips = parsedResponse.RecommendedActions ?? new List<string>(),
            Resources = resources.Select(r => new MedicalResourceDto
            {
                Title = r.Title,
                Url = r.Url,
                Source = r.Source,
                Description = r.Description
            }).ToList(),
            UrgencyLevel = parsedResponse.UrgencyLevel ?? "Low",
            ModelUsed = agentResponse.ModelUsed,
            ProcessingTimeMs = (int)stopwatch.ElapsedMilliseconds
        };
        //analysis result was return to user the agent response what patient was needed
    }

    private async Task<Conversation> GetOrCreateConversationAsync(string userId, SymptomAnalysisRequest request)
    {
        if (request.ConversationId.HasValue)
        {
            return await _dbContext.Conversations
                .FirstAsync(c => c.Id == request.ConversationId.Value && c.UserId == userId);
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = userId,//user info
            Title = $"Symptom Check - {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            UrgencyLevel = "Low",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Conversations.Add(conversation);
        return conversation;
        //store urgency level base user info store in database
        //like this user have a urgency-level=HIGH and this user have a urgency-level=LOW
        //easy to define the urgency level of the conversation
    }

    private static ParsedAgentResponse ParseAgentResponse(string content)
    {
        var json = content;

        var jsonStart = content.IndexOf('{');
        var jsonEnd = content.LastIndexOf('}');
        if (jsonStart >= 0 && jsonEnd > jsonStart)
        {
            json = content[jsonStart..(jsonEnd + 1)];
        }

        try
        {
            return JsonSerializer.Deserialize<ParsedAgentResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new ParsedAgentResponse();
        }
        catch
        {
            return new ParsedAgentResponse
            {
                RawContent = content,
                UrgencyLevel = "Low"
            };
        }
    }
}
