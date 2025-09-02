using Microsoft.EntityFrameworkCore;
using model;

namespace dataaccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Description);
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false);
            entity.Property(e => e.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasColumnType("timestamp");
        });
    }
}

