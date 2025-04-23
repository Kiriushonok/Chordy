using Chordy.BusinessLogic.Models;

namespace Chordy.BusinessLogic.Validators
{
    public static class ChordValidator
    {
        public static void Validate(ChordCreateDto chordCreateDto)
        {
            if (chordCreateDto == null)
                throw new ArgumentNullException(nameof(chordCreateDto), "Данные аккорда не указаны.");

            if (string.IsNullOrWhiteSpace(chordCreateDto.Name))
                throw new ArgumentException("Название аккорда не может быть пустым или состоять только из пробелов.");

            if (chordCreateDto.Name.Length > 30)
                throw new ArgumentException("Название аккорда не должно превышать 30 символов.");

            if (chordCreateDto.Name.Length < 1)
                throw new ArgumentException("Название аккорда не должно быть пустым");
        }
    }
}
