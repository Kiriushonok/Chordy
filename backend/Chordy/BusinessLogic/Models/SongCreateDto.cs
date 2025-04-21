using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class SongCreateDto
    {
        [Required(ErrorMessage = "Название песни обязательно")]
        [MaxLength(100, ErrorMessage = "Название песни не должно превышать 100 символов.")]
        [MinLength(1, ErrorMessage = "Название песни не должно быть пустым")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Текст песни обязателен")]
        [MaxLength(15000, ErrorMessage = "Текст песни не должен превышать 15000 символов.")]
        [MinLength(50, ErrorMessage = "Текст песни не может быть меньше 50 символов (без пробелов).")]
        public required string Text { get; set; }
        public Guid UserId { get; set; }
        public bool IsPublic { get; set; } = true;
        public List<int> AuthorIds { get; set; } = new List<int>();
        public List<int> CollectionIds { get; set; } = new List<int>();
    }
}
