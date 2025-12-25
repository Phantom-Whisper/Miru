using Microsoft.EntityFrameworkCore;
using Miru.Domain;

namespace Miru.Infrastructure
{
    public class MiruContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<UserMedia> UserMedias => Set<UserMedia>();
        public DbSet<SeasonEntity> Seasons => Set<SeasonEntity>();
        public DbSet<EpisodeEntity> Episodes => Set<EpisodeEntity>();

        public MiruContext() 
        { }

        public MiruContext(DbContextOptions<MiruContext> options) 
            : base(options) 
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MiruDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MediaEntity>()
                .HasDiscriminator<string>("MediaType")
                .HasValue<MovieEntity>("Movie")
                .HasValue<SerieEntity>("Series");

            modelBuilder.Entity<UserMedia>()
                .HasKey(um => new { um.UserId, um.MediaId });
        }
    }
}
