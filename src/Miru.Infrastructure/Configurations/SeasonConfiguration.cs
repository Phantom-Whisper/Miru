using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain;

namespace Miru.Infrastructure.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.SeasonNumber)
            .IsRequired();
        
        builder.Property(s => s.ReleaseDate)
            .IsRequired();
        
        builder.HasMany(s => s.Episodes)
            .WithOne(episode => episode.Season)
            .HasForeignKey(episode => episode.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(s => new { s.SerieId, s.SeasonNumber }).IsUnique();
    }
}