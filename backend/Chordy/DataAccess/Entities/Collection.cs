namespace Chordy.DataAccess.Entities
{
    public class Collection
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<SongCollection> SongCollections { get; set; } = new List<SongCollection>();
    }
}
