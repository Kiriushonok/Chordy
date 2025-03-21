using Chordy.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess
{
    public class ChordyDbContext(DbContextOptions<ChordyDbContext> options) : DbContext(options)
    {
        public DbSet<Author> authors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasKey(x => x.id);
            modelBuilder.Entity<Author>().Property(x => x.name).HasMaxLength(30);
            modelBuilder.Entity<Author>().Property(x => x.name).IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}
