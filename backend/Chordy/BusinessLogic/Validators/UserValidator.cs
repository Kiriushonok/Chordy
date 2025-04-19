using Chordy.BusinessLogic.Models;

namespace Chordy.BusinessLogic.Validators
{
    public class UserValidator
    {
        public static void Validate(UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto == null) 
            {
                throw new ArgumentNullException(nameof(userRegisterDto), "Данные пользователя не могут быть null.");
            }

            if (string.IsNullOrWhiteSpace(userRegisterDto.Login))
            {
                throw new ArgumentException("Логин пользователя не может быть пустым или состоять только из пробелов.");
            }

            if (string.IsNullOrWhiteSpace(userRegisterDto.Password))
            {
                throw new ArgumentException("Пароль пользователя не может быть пустым или состоять только из пробелов.");
            }

            if (userRegisterDto.Login.Length > 30)
                throw new ArgumentException("Логин пользователя не должен превышать 30 символов.");

            if (userRegisterDto.Login.Length < 1)
                throw new ArgumentException("Логин пользователя не должен быть пустым.");


            if (userRegisterDto.Password.Length > 64)
                throw new ArgumentException("Пароль пользователя не должен превышать 64 символов.");

            if (userRegisterDto.Password.Length < 8)
                throw new ArgumentException("Пароль пользователя не должен быть меньше 8 символов");
        }
    }
}
