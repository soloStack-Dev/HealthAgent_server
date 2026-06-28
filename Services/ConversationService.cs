using Microsoft.EntityFrameworkCore;
using MediAgent.Api.Data;
using MediAgent.Api.Models.Responses;

namespace MediAgent.Api.Services;

public class ConversationService : IConversationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(ApplicationDbContext dbContext, ILogger<ConversationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<ConversationSummary>> GetUserConversationsAsync(string userId)
    {
        return await _dbContext.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new ConversationSummary
            {
                Id = c.Id,
                Title = c.Title,
                Summary = c.Summary,
                UrgencyLevel = c.UrgencyLevel,
                MessageCount = c.Messages.Count,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }
//get conversation detail by conversation id and user id
//when user want to get the conversation detail
//the server was response the conversation detail
//one user have a many conversation
    public async Task<ConversationDetail?> GetConversationAsync(Guid conversationId, string userId)
    {
        var conversation = await _dbContext.Conversations
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null) return null;

        return new ConversationDetail
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Summary = conversation.Summary,
            UrgencyLevel = conversation.UrgencyLevel,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            Messages = conversation.Messages.Select(m => new MessageDetail
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                ModelUsed = m.ModelUsed,
                ProcessingTimeMs = m.ProcessingTimeMs,
                CreatedAt = m.CreatedAt
            }).ToList()
        };
    }
    //one conversation have a many message
    //when user want to get the conversation detail
    //the server was response the conversation detail
    //one conversation have a many message
    public async Task<bool> DeleteConversationAsync(Guid conversationId, string userId)
    {
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null) return false;

        _dbContext.Conversations.Remove(conversation);
        await _dbContext.SaveChangesAsync();
        return true;
        //when user want to delete the conversation
        //the server was response the true
        //one conversation have a many message
    }

    public async Task<string?> GenerateTitleAsync(Guid conversationId, string userId)
    {
        var conversation = await _dbContext.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation?.Messages.FirstOrDefault() == null) return null;

        var firstMessage = conversation.Messages.First(m => m.Role == "User");
        var title = firstMessage.Content.Length > 50
            ? firstMessage.Content[..50] + "..."
            : firstMessage.Content;

        conversation.Title = title;
        conversation.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return title;
        //when user want to generate the title
        //the server was response the title
        //one conversation have a many message
    }
}
