using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class AuthorDto
    {
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}
