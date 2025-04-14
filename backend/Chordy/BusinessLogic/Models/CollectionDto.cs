using System.ComponentModel.DataAnnotations;

namespace Chordy.BusinessLogic.Models
{
    public class CollectionDto
    {
            [Required(ErrorMessage = "Название подборки обязательно.")]
            [MaxLength(30, ErrorMessage = "Название подборки не должно превышать 30 символов.")]
            public required string Name { get; set; }
    }
}
