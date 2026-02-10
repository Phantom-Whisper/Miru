using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain;

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
        
        // Indexes
        builder.HasIndex(e => new { e.SeasonId, e.EpisodeNumber }).IsUnique();
    }
}