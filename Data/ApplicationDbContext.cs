using Microsoft.EntityFrameworkCore;
using MediAgent.Api.Entities;

namespace MediAgent.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MedicalResource> MedicalResources => Set<MedicalResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.ConversationConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.MessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
