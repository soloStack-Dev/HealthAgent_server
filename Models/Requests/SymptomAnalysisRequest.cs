namespace MediAgent.Api.Models.Requests;

public class SymptomAnalysisRequest
{
    public string Description { get; set; } = string.Empty;
    public Guid? ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}