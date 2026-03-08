using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain.Entities;

namespace Miru.Infrastructure.Configurations;

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(e => e.Duration)
            .IsRequired();
        
        builder.Property(e => e.EpisodeNumber)
            .IsRequired();
        
        builder.Property(e => e.Watched)
            .IsRequired();
        
        builder.Property(e => e.Rating)
            .HasPrecision(3, 1);
        
        builder.HasOne(e => e.Season)
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(e => new { e.SeasonId, e.EpisodeNumber }).IsUnique();
        builder.HasIndex(e => e.Watched);
    }
}