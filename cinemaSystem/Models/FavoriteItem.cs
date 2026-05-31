namespace cinemaSystem.Models
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public string ApplicationuserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
