using Chordy.BusinessLogic.Models;
using System;

namespace Chordy.BusinessLogic.Validators
{
    public static class AuthorValidator
    {
        public static void Validate(AuthorCreateDto authorCreateDto)
        {
            if (authorCreateDto == null)
                throw new ArgumentNullException(nameof(authorCreateDto), "Данные автора не могут быть null.");

            if (string.IsNullOrWhiteSpace(authorCreateDto.Name))
                throw new ArgumentException("Имя автора не может быть пустым или состоять только из пробелов.");

            if (authorCreateDto.Name.Length > 30)
                throw new ArgumentException("Имя автора не должно превышать 30 символов.");

            if (authorCreateDto.Name.Length < 1)
                throw new ArgumentException("Имя автора не должно быть пустым");
        }
    }
} 