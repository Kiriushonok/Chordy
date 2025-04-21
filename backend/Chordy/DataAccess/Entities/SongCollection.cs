namespace Chordy.DataAccess.Entities
{
    public class SongCollection
    {
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;

        public int CollectionId { get; set; }
        public Collection Collection { get; set; } = null!;
    }
}
