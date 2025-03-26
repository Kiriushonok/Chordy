using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class CollectionDto
    {
            [Required(ErrorMessage = "Название подборки обязательно.")]
            [MaxLength(30, ErrorMessage = "Название подборки не должно превышать 30 символов.")]
            public string Name { get; set; } = string.Empty; // Защита от null
    }
}
