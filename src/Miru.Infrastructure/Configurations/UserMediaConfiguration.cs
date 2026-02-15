using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Miru.Domain;

namespace Miru.Infrastructure.Configurations;
// Commented for now, may be useful in V2
/*
public class UserMediaConfiguration : IEntityTypeConfiguration<UserMedia>
{
    public void Configure(EntityTypeBuilder<UserMedia> builder)
    {
        builder.HasKey(um => new { um.UserId, um.MediaId });
        
        builder.Property(um => um.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(um => um.AddedAt)
            .IsRequired();
        
        builder.Property(um => um.Rating)
            .HasPrecision(2, 1);
        
        // Relationships
        builder.HasOne(um => um.User)
            .WithMany(u => u.UserMedias)
            .HasForeignKey(um => um.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(um => um.Media)
            .WithMany(m => m.UserMedias)
            .HasForeignKey(um => um.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(um => um.AddedAt);
        builder.HasIndex(um => um.Status);
    }
}*/