using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediAgent.Api.Entities;

namespace MediAgent.Api.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.ModelUsed)
            .HasMaxLength(50);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.MedicalResources)
            .WithOne(r => r.Message)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.CreatedAt);
    }
}

/*

//one conversation have many messages
//one message have many medical resources

{
  "id": "msg-001-abc",
  "conversationId": "conv-123-xyz",
  "role": "assistant",
  "content": "Based on your symptoms, you may be experiencing a migraine. Please consult a healthcare professional for a proper diagnosis.",
  "modelUsed": "gpt-4o",
  "createdAt": "2026-06-05T07:59:00Z",
  "medicalResources": [
    {
      "id": "res-001",
      "messageId": "msg-001-abc",
      "title": "Migraine - Symptoms and Causes",
      "url": "https://www.mayoclinic.org/diseases-conditions/migraine",
      "source": "Mayo Clinic"
    }
  ]
}
*/