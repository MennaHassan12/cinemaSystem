using cinemaSystem.Models;

namespace cinemaSystem.ViewModel
{
    public class MovieWithRelatedVM
    {
        public Movie Movie { get; set; } = null!;
        public IEnumerable<Movie> Movies { get; set; } = [];
        public IEnumerable<Category> Categories { get; set; } = [];
       
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? Query { get; set; }
    }
}
