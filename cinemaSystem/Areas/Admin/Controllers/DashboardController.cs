using cinemaSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalMovies = await _context.Movies.CountAsync();
            var totalActors = await _context.Actors.CountAsync();
            var totalCinemas = await _context.Cinemas.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();

            var recentActors = await _context.Actors
                .OrderByDescending(a => a.Id)
                .Take(6)
                .ToListAsync();

            var recentMovies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .OrderByDescending(m => m.Id)
                .Take(6)
                .ToListAsync();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalActors = totalActors;
            ViewBag.TotalCinemas = totalCinemas;
            ViewBag.TotalCategories = totalCategories;

            ViewBag.RecentActors = recentActors;
            ViewBag.RecentMovies = recentMovies;

            return View();
        }
    }
}