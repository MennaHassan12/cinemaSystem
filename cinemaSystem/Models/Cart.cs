namespace cinemaSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationuserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = new Movie();

        public double MoviePrice { get; set; }
        public double TotalPrice { get; set; }
    }
}
