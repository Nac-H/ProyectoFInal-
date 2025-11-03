using Microsoft.EntityFrameworkCore;
using Peliculas.Models;

namespace Peliculas.Data.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Director> Directores { get; set; }
        public DbSet<Genero> Generos { get; set; }
      protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActorPelicula>()
                .HasKey(ap => new { ap.ActorId, ap.PeliculaId });

            modelBuilder.Entity<ActorPelicula>()
                .HasOne(ap => ap.Actor)
                .WithMany(a => a.Peliculas)
                .HasForeignKey(ap => ap.ActorId);   

            modelBuilder.Entity<ActorPelicula>()
                .HasOne(ap => ap.Pelicula)
                .WithMany(p => p.Actores)
                .HasForeignKey(ap => ap.PeliculaId);
        }
    }
}
