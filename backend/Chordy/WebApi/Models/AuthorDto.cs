using System.ComponentModel.DataAnnotations;

namespace Chordy.WebApi.Models
{
    public class AuthorDto
    {
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}
