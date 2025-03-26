using Chordy.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess
{
    public class ChordyDbContext(DbContextOptions<ChordyDbContext> options) : DbContext(options)
    {
        public DbSet<Author> authors { get; set; }
        public DbSet<Collection> collections { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasKey(x => x.id);
            modelBuilder.Entity<Author>().Property(x => x.name).HasMaxLength(30);
            modelBuilder.Entity<Author>().Property(x => x.name).IsRequired();

            modelBuilder.Entity<Collection>().HasKey(x => x.Id);
            modelBuilder.Entity<Collection>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Collection>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Collection>().Property(x => x.Name).IsRequired().HasColumnName("name");

            base.OnModelCreating(modelBuilder);
        }
    }
}
