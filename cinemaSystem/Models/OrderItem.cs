namespace cinemaSystem.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
