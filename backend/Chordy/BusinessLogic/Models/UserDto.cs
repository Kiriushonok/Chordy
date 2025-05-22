using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class UserDto
    {
        [Required(ErrorMessage = "Логин пользователя обязателен")]
        [MaxLength(30, ErrorMessage = "Логин пользователя не должен превышать 30 символов")]
        [MinLength(1, ErrorMessage = "Логин пользователя не может быть меньше 1 символа")]
        public required string Login { get; set; }
        public Guid Id { get; set; }
        public string? Role { get; set; }
    }
}
