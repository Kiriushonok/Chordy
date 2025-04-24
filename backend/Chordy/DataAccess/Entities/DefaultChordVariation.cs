namespace Chordy.DataAccess.Entities
{
    public class DefaultChordVariation
    {
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;

        public int ChordVariationId { get; set; }
        public ChordVariation ChordVariation { get; set; } = null!;
    }
}
