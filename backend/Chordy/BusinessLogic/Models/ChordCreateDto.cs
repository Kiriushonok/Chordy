using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class ChordCreateDto
    {
        [Required(ErrorMessage = "Название аккорда обязательно")]
        [MaxLength(30, ErrorMessage = "Название аккорда не должно превышать 30 символов")]
        [MinLength(1, ErrorMessage = "Название аккорда не должно быть пустым")]
        public required string Name { get; set; }
    }
}
