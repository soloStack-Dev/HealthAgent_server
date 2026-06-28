using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediAgent.Api.Entities;

namespace MediAgent.Api.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Title)
            .HasMaxLength(200);

        builder.Property(c => c.UrgencyLevel)
            .HasMaxLength(20)
            .HasDefaultValue("Low");

        builder.Property(c => c.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CreatedAt);
    }
}
