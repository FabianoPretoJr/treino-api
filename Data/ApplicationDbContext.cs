using Microsoft.EntityFrameworkCore;
using projeto.Models;

namespace projeto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Criminoso> criminosos { get; set; }
        public DbSet<Vitima> vitimas { get; set; }
        public DbSet<Legista> legistas { get; set; }
        public DbSet<Policial> policiais { get; set; }
        public DbSet<Delegacia> delegacias { get; set; }
        public DbSet<Delegado> delegados { get; set; }
        public DbSet<Crime> crimes { get; set; }
        public DbSet<Autopsia> autopsias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crime>().HasKey(sc => new { sc.CriminosoID, sc.VitimaID, sc.PolicialID});
            modelBuilder.Entity<Autopsia>().HasKey(sc => new { sc.VitimaID, sc.LegistaID});
            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {
        }
    }
}