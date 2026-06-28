using MediAgent.Api.Models.Responses;

namespace MediAgent.Api.Services;

public interface IConversationService
{
    Task<List<ConversationSummary>> GetUserConversationsAsync(string userId);
    Task<ConversationDetail?> GetConversationAsync(Guid conversationId, string userId);
    Task<bool> DeleteConversationAsync(Guid conversationId, string userId);
    Task<string?> GenerateTitleAsync(Guid conversationId, string userId);
}

public class ConversationDetail
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string UrgencyLevel { get; set; } = "Low";
    public List<MessageDetail> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MessageDetail
{
    public Guid Id { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ModelUsed { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
