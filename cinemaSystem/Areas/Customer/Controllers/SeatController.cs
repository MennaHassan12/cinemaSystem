using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Customer.Controllers
{
    public class SeatController : Controller
    {
        public IActionResult SelectSeats(int movieId)
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
