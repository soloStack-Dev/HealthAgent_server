using MediAgent.Api.Common;
using MediAgent.Api.Services;

namespace MediAgent.Api.Endpoints;

public static class ConversationEndpoints
{
    public static void MapConversationEndpoints(this WebApplication app)
    {
        var conversationGroup = app.MapGroup("/api/conversations");

        conversationGroup.MapGet("/", async (string userId, IConversationService conversationService) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.BadRequest(ApiResponse<object>.Fail("UserId is required"));

            var conversations = await conversationService.GetUserConversationsAsync(userId);
            return Results.Ok(ApiResponse<object>.Ok(conversations));
        })
        .WithName("ListConversations")
        .WithOpenApi();

        conversationGroup.MapGet("/{id:guid}", async (Guid id, string userId, IConversationService conversationService) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.BadRequest(ApiResponse<object>.Fail("UserId is required"));

            var conversation = await conversationService.GetConversationAsync(id, userId);
            if (conversation == null)
                return Results.NotFound(ApiResponse<object>.Fail("Conversation not found"));

            return Results.Ok(ApiResponse<object>.Ok(conversation));
        })
        .WithName("GetConversation")
        .WithOpenApi();

        conversationGroup.MapDelete("/{id:guid}", async (Guid id, string userId, IConversationService conversationService) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.BadRequest(ApiResponse<object>.Fail("UserId is required"));

            var deleted = await conversationService.DeleteConversationAsync(id, userId);
            if (!deleted)
                return Results.NotFound(ApiResponse<object>.Fail("Conversation not found"));

            return Results.Ok(ApiResponse<object>.Ok(null, "Conversation deleted"));
        })
        .WithName("DeleteConversation")
        .WithOpenApi();

        conversationGroup.MapPost("/{id:guid}/title", async (Guid id, string userId, IConversationService conversationService) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.BadRequest(ApiResponse<object>.Fail("UserId is required"));

            var title = await conversationService.GenerateTitleAsync(id, userId);
            if (title == null)
                return Results.NotFound(ApiResponse<object>.Fail("Conversation not found"));

            return Results.Ok(ApiResponse<object>.Ok(new { title }, "Title generated"));
        })
        .WithName("GenerateTitle")
        .WithOpenApi();
    }
}