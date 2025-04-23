namespace Chordy.BusinessLogic.Models
{
    public class StringPress
    {
        public int Fret { get; set; }
        public int Finger { get; set; }
    }

    public class StringState
    {
        public string State { get; set; } = null!; // "open", "mute", "pressed"
        public List<StringPress> Presses { get; set; } = new();
    }

    public class ApplicaturaModel
    {
        public List<StringState> Strings { get; set; } = new();
    }
}
