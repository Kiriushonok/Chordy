using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chordy.DataAccess.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? AvatarPath { get; set; }
        public ICollection<SongAuthor> SongAuthors { get; set; } = new List<SongAuthor>();
    }
}
