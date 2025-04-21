namespace Chordy.DataAccess.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Text { get; set; }
        public int Views { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsPublic { get; set; } = true;
        public ICollection<SongAuthor> SongAuthors { get; set; } = new List<SongAuthor>();
        public ICollection<SongCollection> SongCollections { get; set; } = new List<SongCollection>();
    }
}
