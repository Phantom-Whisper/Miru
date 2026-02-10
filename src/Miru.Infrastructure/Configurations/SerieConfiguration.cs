using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain;

namespace Miru.Infrastructure.Configurations;

public class SerieConfiguration : IEntityTypeConfiguration<Serie>
{
    public void Configure(EntityTypeBuilder<Serie> builder)
    {
        builder.HasMany(s => s.Seasons)
            .WithOne(s => s.Serie)
            .HasForeignKey(season => season.SerieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}