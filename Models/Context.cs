using Microsoft.EntityFrameworkCore;
using System;

namespace TestFilms.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) 
        {
        
        }
    
        public DbSet<Category> Categories { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<FilmCategory> FilmCategory { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>()
                .HasMany(e => e.Categories)
                .WithMany(e => e.Films)
                .UsingEntity<FilmCategory>();
        }
    }
}
