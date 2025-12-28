using Microsoft.EntityFrameworkCore;
using Miru.Domain;

namespace Miru.Infrastructure
{
    public class MiruContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<UserMedia> UserMedias => Set<UserMedia>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<Episode> Episodes => Set<Episode>();

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

            modelBuilder.Entity<Media>()
                .HasDiscriminator<string>("MediaType")
                .HasValue<Movie>("Movie")
                .HasValue<Serie>("Series");

            modelBuilder.Entity<UserMedia>()
                .HasKey(um => new { um.UserId, um.MediaId });
        }
    }
}
