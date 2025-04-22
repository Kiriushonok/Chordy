using Chordy.BusinessLogic.Models;
using System;

namespace Chordy.BusinessLogic.Validators
{
    public static class CollectionValidator
    {
        public static void Validate(CollectionCreateDto collectionCreateDto)
        {
            if (collectionCreateDto == null)
                throw new ArgumentNullException(nameof(collectionCreateDto), "Данные подборки не указаны.");

            if (string.IsNullOrWhiteSpace(collectionCreateDto.Name))
                throw new ArgumentException("Имя подборки не может быть пустым или состоять только из пробелов.");

            if (collectionCreateDto.Name.Length > 30)
                throw new ArgumentException("Имя подборки не должно превышать 30 символов.");

            if (collectionCreateDto.Name.Length < 1)
                throw new ArgumentException("Имя подборки не должно быть пустым");
        }
    }
} 