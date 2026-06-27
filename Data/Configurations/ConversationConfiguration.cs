using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediAgent.Api.Entities;

namespace MediAgent.Api.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200);

        builder.Property(c => c.UrgencyLevel)
            .HasMaxLength(20)
            .HasDefaultValue("Low");

        builder.Property(c => c.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Conversations)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CreatedAt);
    }
}

/*

//one conersation have one user
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "title": "Chest pain and shortness of breath",
  "urgencyLevel": "High",
  "createdAt": "2026-06-05T07:34:00Z",
  "updatedAt": "2026-06-05T07:56:00Z",
  "userId": "user-123-456" relationship with user table
}
// one user have many conversations
{
  "id": "user-123-456",
  "email": "patient@example.com",
  "name": "John Doe",
  "conversations": [
    {
      "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
      "title": "Chest pain and shortness of breath",
      "urgencyLevel": "High",
      "createdAt": "2026-06-05T07:34:00Z",
      "updatedAt": "2026-06-05T07:56:00Z"
    },
    {
      "id": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
      "title": "Headache and fever follow-up",
      "urgencyLevel": "Low",
      "createdAt": "2026-06-04T14:20:00Z",
      "updatedAt": "2026-06-04T15:10:00Z"
    }
  ]
}
*/