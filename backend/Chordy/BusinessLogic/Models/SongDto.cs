namespace Chordy.BusinessLogic.Models
{
    public class SongDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Text { get; set; }
        public int Views { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public required string UserLogin { get; set; }
        public bool IsPublic { get; set; }
        public int FavouritesCount { get; set; }
        public List<AuthorDto> Authors { get; set; } = new();
        public List<CollectionDto> Collections { get; set; } = new();
    }
}
