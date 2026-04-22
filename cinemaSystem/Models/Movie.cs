using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinemaSystem.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie name is required")]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool Status { get; set; } = true;

        public DateTime DateTime { get; set; } = DateTime.Now;

        public string? MainImage { get; set; }

        public List<string>? SubImages{ get; set; } 

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("Cinema")]
        public int CinemaId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Cinema? Cinema { get; set; }

        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        public virtual Movie? Movie { get; set; }
        public virtual Actor? Actor { get; set; }
    }
}
