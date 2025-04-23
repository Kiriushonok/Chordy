using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class ChordVariationCreateDto
    {
        [Required(ErrorMessage = "Не указан аккорд.")]
        public int ChordId { get; set; }
        [Required(ErrorMessage = "Не указана аппликатура.")]
        public ApplicaturaModel Applicatura { get; set; } = null!;
        [Range(1, 24, ErrorMessage = "Лад должен быть в диапазоне от 1 до 20.")]
        public int StartFret { get; set; }
        public bool Bare { get; set; }
        [Required(ErrorMessage = "SVG-картинка обязательна.")]
        public string FingeringSVG { get; set; } = null!;
    }
}
