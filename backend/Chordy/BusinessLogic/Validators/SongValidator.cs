using Chordy.BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace Chordy.BusinessLogic.Validators
{
    public static class SongValidator
    {
        public static void Validate(SongCreateDto songCreateDto)
        {
            if (songCreateDto == null)
                throw new ArgumentNullException(nameof(songCreateDto), "Данные песни не указаны.");

            if (string.IsNullOrWhiteSpace(songCreateDto.Name))
                throw new ArgumentException("Название песни не может быть пустым или состоять только из пробелов.");

            if (songCreateDto.Name.Length > 100)
                throw new ArgumentException("Название песни не должно превышать 100 символов.");

            if (songCreateDto.Name.Length < 1)
                throw new ArgumentException("Название песни не должно быть пустым.");

            if (string.IsNullOrWhiteSpace(songCreateDto.Text))
                throw new ArgumentException("Текст песни не может быть пустым или состоять только из пробелов.");

            var textWithoutSpaces = songCreateDto.Text.Replace(" ", "");
            if (textWithoutSpaces.Length < 50)
                throw new ArgumentException("Текст песни не может быть меньше 50 символов (без пробелов).");

            if (songCreateDto.Text.Length > 15000)
                throw new ArgumentException("Текст песни не должен превышать 15000 символов.");
        }
    }
} 