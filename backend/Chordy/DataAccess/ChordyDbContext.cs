using Chordy.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess
{
    public class ChordyDbContext(DbContextOptions<ChordyDbContext> options) : DbContext(options)
    {
        public DbSet<Author> authors { get; set; }
        public DbSet<Collection> collections { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<Song> songs { get; set; }
        public DbSet<SongAuthor> songAuthors { get; set; }
        public DbSet<SongCollection> songCollections { get; set; }
        public DbSet<SongViews> songViews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Авторы
            modelBuilder.Entity<Author>().HasKey(x => x.Id);
            modelBuilder.Entity<Author>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Author>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Author>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Author>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Author_Name_NotEmpty", "length(trim(\"Name\")) > 0");
                });

            // Подборки
            modelBuilder.Entity<Collection>().HasKey(x => x.Id);
            modelBuilder.Entity<Collection>().Property(x => x.Name).HasMaxLength(30);
            modelBuilder.Entity<Collection>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Collection>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Collection>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Collection_Name_NotEmpty", "length(trim(\"Name\")) > 0");
                });

            // Пользователи
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

            // Роли
            modelBuilder.Entity<Role>().HasKey(x => x.Id);
            modelBuilder.Entity<Role>().Property(x => x.Name).IsRequired().HasMaxLength(30);
            modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();

            // Пользователь - роль
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Песни
            modelBuilder.Entity<Song>().HasIndex(x => x.Id);
            modelBuilder.Entity<Song>().Property(x => x.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Song>().Property(x => x.Text).HasMaxLength(15000).IsRequired();
            modelBuilder.Entity<Song>().Property(x => x.Views).HasDefaultValue(0);
            modelBuilder.Entity<Song>().Property(x => x.Date).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            modelBuilder.Entity<Song>().Property(x => x.IsPublic).HasDefaultValue(true);

            modelBuilder.Entity<Song>().ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Song_Text_NotTooSmall", "length(trim(\"Text\")) > 50");
            });

            modelBuilder.Entity<Song>().ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Song_Name_NotEmpty", "length(trim(\"Name\")) > 0");
            });

            modelBuilder.Entity<Song>()
                .HasOne(x => x.User)
                .WithMany(u => u.Songs)
                .HasForeignKey(x => x.UserId);

            // Песня - Автор
            modelBuilder.Entity<SongAuthor>().HasKey(sa => new { sa.SongId, sa.AuthorId });

            modelBuilder.Entity<SongAuthor>()
                .HasOne(sa => sa.Song)
                .WithMany(s => s.SongAuthors)
                .HasForeignKey(sa => sa.SongId);

            modelBuilder.Entity<SongAuthor>()
                .HasOne(sa => sa.Author)
                .WithMany(a => a.SongAuthors)
                .HasForeignKey(sa => sa.AuthorId);

            // Песня - Подборка
            modelBuilder.Entity<SongCollection>().HasKey(sc => new { sc.SongId, sc.CollectionId });

            modelBuilder.Entity<SongCollection>()
                .HasOne(sc => sc.Song)
                .WithMany(s => s.SongCollections)
                .HasForeignKey(sa => sa.SongId);

            modelBuilder.Entity<SongCollection>()
                .HasOne(sc => sc.Collection)
                .WithMany(c => c.SongCollections)
                .HasForeignKey(sc => sc.CollectionId);

            // Песня - просмотр пользователя
            modelBuilder.Entity<SongViews>().HasKey(sw => new { sw.SongId, sw.UserId });

            modelBuilder.Entity<SongViews>()
                .HasOne(sw => sw.User)
                .WithMany()
                .HasForeignKey(sw => sw.UserId);

            modelBuilder.Entity<SongViews>()
                .HasOne(sw => sw.Song)
                .WithMany()
                .HasForeignKey(sw => sw.SongId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
