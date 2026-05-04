using cinemaSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalMovies = await _dashboardService.GetMoviesCount();
            ViewBag.TotalActors = await _dashboardService.GetActorsCount();
            ViewBag.TotalCinemas = await _dashboardService.GetCinemasCount();
            ViewBag.TotalCategories = await _dashboardService.GetCategoriesCount();

            ViewBag.RecentActors = await _dashboardService.GetRecentActors();
            ViewBag.RecentMovies = await _dashboardService.GetRecentMovies();

            return View();
        }
    }
}