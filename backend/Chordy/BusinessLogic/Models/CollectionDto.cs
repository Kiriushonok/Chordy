using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class CollectionDto
    {
        [Required(ErrorMessage = "Название подборки обязательно.")]
        [MaxLength(30, ErrorMessage = "Название подборки не должно превышать 30 символов.")]
        [MinLength(1, ErrorMessage = "Название коллекции не должно быть пустым")]
        public required string Name { get; set; }
    }
}
