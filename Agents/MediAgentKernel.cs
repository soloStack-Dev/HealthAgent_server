using System.Text.Json;
using MediAgent.Api.Infrastructure.Ai;
using MediAgent.Api.Models.Responses;

namespace MediAgent.Api.Agents;

public class MediAgentKernel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MediAgentKernel> _logger;

    public MediAgentKernel(IConfiguration configuration, ILogger<MediAgentKernel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AgentResponse> AnalyzeSymptomsAsync(string userPrompt)
    {
        try
        {
            var groqApiKey = _configuration["Groq:ApiKey"] ?? "";
            var groqModel = _configuration["Groq:Model"] ?? "llama-3.3-70b-versatile";
            var groqBaseUrl = _configuration["Groq:BaseUrl"] ?? "https://api.groq.com/openai/v1";
            var timeoutSeconds = int.Parse(_configuration["Groq:TimeoutSeconds"] ?? "120");
            var maxTokens = int.Parse(_configuration["Groq:MaxTokens"] ?? "2048");
            var temperature = float.Parse(_configuration["Groq:Temperature"] ?? "0.3");

            using var groqClient = new GroqClient(groqApiKey, groqModel, groqBaseUrl, timeoutSeconds);

            var systemPrompt = @"You are MediAgent, an intelligent health symptom analyzer.
You help users understand their symptoms by providing preliminary assessments,
health tips, and links to verified medical resources.
You NEVER provide definitive diagnoses or prescribe medications.
Always include a medical disclaimer in every response.

Respond ONLY with valid JSON. Do not include any explanation, markdown formatting, or extra text.

Example output:
{
  ""possibleConditions"": [
    {
      ""name"": ""Tension Headache"",
      ""confidence"": ""Medium"",
      ""description"": ""A common type of headache often triggered by stress or muscle tension"",
      ""matchingSymptoms"": [""headache"", ""neck tension"", ""pressure around forehead""]
    }
  ],
  ""urgencyLevel"": ""Low"",
  ""recommendedActions"": [
    ""Rest in a quiet, dark room"",
    ""Stay hydrated"",
    ""Apply a cold or warm compress to the forehead""
  ],
  ""questionsToAsk"": [
    ""How long have you had these symptoms?"",
    ""Have you taken any medication for it?""
  ]
}

IMPORTANT: If symptoms indicate EMERGENCY (chest pain, difficulty breathing, severe bleeding, loss of consciousness), immediately set urgencyLevel to 'Emergency' and advise calling emergency services.";

            var response = await groqClient.ChatAsync(systemPrompt, userPrompt, maxTokens, temperature);

            _logger.LogInformation("Groq response received for symptom analysis");

            return new AgentResponse
            {
                Content = response,
                ModelUsed = groqModel,
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze symptoms with Groq");

            var fallbackContent = JsonSerializer.Serialize(new
            {
                possibleConditions = Array.Empty<object>(),
                urgencyLevel = "Low",
                recommendedActions = new[] { "Please consult a healthcare professional for proper evaluation" },
                questionsToAsk = Array.Empty<string>(),
                rawContent = $"Error: {ex.Message}"
            });

            return new AgentResponse
            {
                Content = fallbackContent,
                ModelUsed = "fallback",
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}
