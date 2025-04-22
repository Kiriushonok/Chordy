namespace Chordy.DataAccess.Entities
{
    public class SongViews
    {
        public Guid UserId { get; set; }
        public int SongId { get; set; }

        public User User { get; set; } = null!;
        public Song Song { get; set; } = null!;
    }
}
