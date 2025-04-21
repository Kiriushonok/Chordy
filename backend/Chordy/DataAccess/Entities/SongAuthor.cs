namespace Chordy.DataAccess.Entities
{
    public class SongAuthor
    {
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
    }
}
