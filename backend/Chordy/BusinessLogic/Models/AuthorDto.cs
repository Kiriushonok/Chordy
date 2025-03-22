using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class AuthorDto
    {
        [Required(ErrorMessage = "Имя автора обязательно.")]
        [MaxLength(30, ErrorMessage = "Имя автора не должно превышать 30 символов.")]
        public string Name { get; set; } = string.Empty; // Защита от null
    }
}
