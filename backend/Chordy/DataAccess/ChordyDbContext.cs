﻿using Chordy.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess
{
    public class ChordyDbContext(DbContextOptions<ChordyDbContext> options) : DbContext(options)
    {
        public DbSet<Author> authors { get; set; }
        public DbSet<Collection> collections { get; set; }
        public DbSet<User> users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasKey(x => x.Id);
            modelBuilder.Entity<Author>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Author>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Author>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Author>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Author_Name_NotEmpty", "length(trim(\"Name\")) > 0");
                });


            modelBuilder.Entity<Collection>().HasKey(x => x.Id);
            modelBuilder.Entity<Collection>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Collection>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Collection>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Collection>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Collection_Name_NotEmpty", "length(trim(\"Name\")) > 0");
                });

            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().Property(x => x.Login).HasMaxLength(30);
            modelBuilder.Entity<User>().Property(x => x.Login).IsRequired();
            modelBuilder.Entity<User>().HasIndex(x => x.Login).IsUnique();
            modelBuilder.Entity<User>().ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_User_Login_NotEmpty", "length(trim(\"Login\")) > 0");
            });
            modelBuilder.Entity<User>().Property(x => x.PasswordHash).IsRequired();
            modelBuilder.Entity<User>().Property(x => x.PasswordHash).HasMaxLength(128);


            base.OnModelCreating(modelBuilder);
        }
    }
}
