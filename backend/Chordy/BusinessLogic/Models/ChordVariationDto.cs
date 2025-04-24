namespace Chordy.BusinessLogic.Models
{
    public class ChordVariationDto
    {
        public int Id { get; set; }
        public int ChordId { get; set; }
        public Guid UserId { get; set; }
        public ApplicaturaModel Applicatura { get; set; } = null!;
        public int StartFret { get; set; }
        public bool Bare { get; set; }
        public string FingeringSVG { get; set; } = null!;
    }
}
