namespace Chordy.BusinessLogic.Models
{
    public class PopularAuthorDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? AvatarPath { get; set; }
        public int TotalViews { get; set; } = 0;
    }
}