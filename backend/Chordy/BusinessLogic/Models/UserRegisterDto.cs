using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Логин пользователя обязателен")]
        [MaxLength(30, ErrorMessage = "Логин пользователя не должен превышать 30 символов")]
        [MinLength(1, ErrorMessage = "Логин пользователя не может быть меньше 1 символа")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Пароль пользователя обязателен")]
        [MaxLength(64, ErrorMessage = "Пароль пользователя не может быть длиннее 64 символов")]
        [MinLength(8, ErrorMessage = "Пароль пользователя не может быть короче 8 символов")]
        public required string Password { get; set; }
    }
}
