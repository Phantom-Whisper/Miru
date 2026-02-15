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
        
        
        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(m => m.AddedAt)
            .IsRequired();
        
        builder.Property(m => m.Rating)
            .HasPrecision(3, 1);

        builder.HasOne(m => m.User)
            .WithMany(u => u.Media)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasDiscriminator<string>("MediaType")
            .HasValue<Movie>("Movie")
            .HasValue<Serie>("Serie");
        
        // Indexes
        builder.HasIndex(m => m.UserId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.AddedAt);
        builder.HasIndex(m => m.Title);
        builder.HasIndex(m => m.ReleaseDate);
    }
}