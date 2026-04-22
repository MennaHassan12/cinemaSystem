using System.ComponentModel.DataAnnotations;

namespace cinemaSystem.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
