namespace Chordy.DataAccess.Entities
{
    public class ChordVariation
    {
        public int Id { get; set; }
        public int ChordId { get; set; }
        public Guid UserId { get; set; }
        public required string Applicatura { get; set; }
        public int StartFret { get; set; }
        public bool Bare { get; set; }
        public required byte[] FingeringSVG { get; set; }

        public Chord Chord { get; set; } = null!;
    }
}
