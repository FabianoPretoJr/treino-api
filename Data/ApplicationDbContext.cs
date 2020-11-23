using System;
using System.Collections.Generic;
using System.Text;
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
        public DbSet<Prissao> prissoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crime>().HasKey(sc => new { sc.CriminosoID, sc.VitimaID});
            modelBuilder.Entity<Autopsia>().HasKey(sc => new { sc.VitimaID, sc.LegistaID});
            modelBuilder.Entity<Prissao>().HasKey(sc => new { sc.PolicialID, sc.CrimosoID, sc.VitimaID});
            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {
        }
    }
}