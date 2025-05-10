using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class AuthorDto
    {
        [Required(ErrorMessage = "Имя автора обязательно.")]
        [MaxLength(30, ErrorMessage = "Имя автора не должно превышать 30 символов.")]
        [MinLength(1, ErrorMessage = "Имя автора не должно быть пустым")]
        public required string Name { get; set; }
        public int Id { get; set; }
        public string? AvatarPath { get; set; }
            public int TotalViews { get; set; } = 0;
    }
}