using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class AuthorCreateDto
    {
        [Required(ErrorMessage = "Имя автора обязательно.")]
        [MaxLength(30, ErrorMessage = "Имя автора не должно превышать 30 символов.")]
        [MinLength(1, ErrorMessage = "Имя автора не должно быть пустым")]
        public required string Name { get; set; }
    }
}
