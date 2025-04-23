using Chordy.BusinessLogic.Models;
using System.ComponentModel.DataAnnotations;

public static class ChordVariationValidator
{
    public static void Validate(ChordVariationCreateDto dto)
    {
        if (dto.Applicatura == null || dto.Applicatura.Strings == null || dto.Applicatura.Strings.Count == 0)
            throw new ArgumentNullException("Аппликатура не может быть пустой.");

        if (string.IsNullOrWhiteSpace(dto.FingeringSVG))
            throw new ArgumentException("SVG-картинка обязательна.");

        if (string.IsNullOrWhiteSpace(dto.FingeringSVG) || !dto.FingeringSVG.TrimStart().StartsWith("<svg"))
            throw new ArgumentException("SVG-картинка должна быть валидным SVG.");
    }
}