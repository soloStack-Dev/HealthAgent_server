using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediAgent.Api.Entities;

namespace MediAgent.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasMaxLength(20);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(u => u.UpdatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);
    }
}
