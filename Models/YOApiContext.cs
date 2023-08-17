using Microsoft.EntityFrameworkCore;
using YOApi.Models.Messages;

namespace YOApi.Models;

public class YOApiContext : DbContext
{
    public YOApiContext(DbContextOptions<YOApiContext> options) : base(options)
    {

    }

#nullable disable
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
#nullable enable

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(
            j =>
            {
                j.Property(p => p.Address).HasMaxLength(50);
                j.Property(p => p.Password).HasMaxLength(50);
                j.HasIndex(e => e.Address).IsUnique();
            });
        modelBuilder
            .Entity<Message>()
            .HasMany(c => c.Users)
            .WithMany(s => s.Messages)
            .UsingEntity<UserMessage>(
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.UserMessages)
                    .HasForeignKey(pt => pt.UserId), 
                j => j
                    .HasOne(pt => pt.Message)
                    .WithMany(p => p.UserMessages)
                    .HasForeignKey(pt => pt.MessageId), 
                j =>
                {
                    j.HasKey(p => p.Id);
                    j.Property(pt => pt.IsArchive).HasDefaultValue(false);
                    j.Property(pt => pt.IsRead).HasDefaultValue(false);
                    j.Property(pt => pt.Role);
                    j.HasAlternateKey(t => new { t.MessageId, t.UserId, t.Role}); 
                    j.ToTable("UserMessage");
                });      
    }
}