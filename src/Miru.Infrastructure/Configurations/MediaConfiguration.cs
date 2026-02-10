using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain;

namespace Miru.Infrastructure.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.PosterUrl)
            .HasMaxLength(1000);
        
        builder.Property(m => m.ReleaseDate)
            .IsRequired();

        builder.HasDiscriminator<string>("MediaType")
            .HasValue<Movie>("Movie")
            .HasValue<Serie>("Serie");
        
        // Indexes
        builder.HasIndex(m => m.Title);
        builder.HasIndex(m => m.ReleaseDate);
    }
}