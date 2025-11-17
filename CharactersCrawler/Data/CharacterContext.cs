using Microsoft.EntityFrameworkCore;
using CharactersCrawler.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CharactersCrawler.Data;

public class CharacterContext : DbContext
{
    public CharacterContext(DbContextOptions<CharacterContext> options) : base(options)
    {
    }

    public DbSet<Character> Characters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Character entity
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Species).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Gender).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Image).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Created).IsRequired();

            // Configure owned entity types for Origin and Location
            entity.OwnsOne(e => e.Origin, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(o => o.Name).IsRequired().HasMaxLength(100);
                ownedNavigationBuilder.Property(o => o.Url).IsRequired().HasMaxLength(200);
            });

            entity.OwnsOne(e => e.Location, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(l => l.Name).IsRequired().HasMaxLength(100);
                ownedNavigationBuilder.Property(l => l.Url).IsRequired().HasMaxLength(200);
            });

            // Configure Episode as a JSON column or separate table
            entity.Property(e => e.Episode)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Metadata.SetValueComparer(new ValueComparer<string[]>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()));
        });
    }
}