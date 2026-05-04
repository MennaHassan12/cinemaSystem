using cinemaSystem.Data;
using cinemaSystem.Models;
using cinemaSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieService _movieService;

        public MoviesController(ApplicationDbContext context, IMovieService movieService)
        {
            _context = context;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var movies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .ToListAsync(ct);

            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync(ct);
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync(ct);
            ViewBag.Actors = await _context.Actors.ToListAsync(ct);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Movie movie,
            IFormFile? mainImageFile,
            List<IFormFile>? subImages,
            int[] selectedActors,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync(ct);
                ViewBag.Cinemas = await _context.Cinemas.ToListAsync(ct);
                ViewBag.Actors = await _context.Actors.ToListAsync(ct);
                return View(movie);
            }

            await _movieService.CreateMovie(movie, mainImageFile, subImages, selectedActors);

            TempData["Success"] = "Movie added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            if (movie == null)
                return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync(ct);
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync(ct);
            ViewBag.Actors = await _context.Actors.ToListAsync(ct);

            ViewBag.SelectedActors = movie.MovieActors.Select(a => a.ActorId).ToArray();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Movie movie,
            IFormFile? mainImageFile,
            List<IFormFile>? subImages,
            int[] selectedActors,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(movie);

            await _movieService.UpdateMovie(movie, id, mainImageFile, subImages, selectedActors);

            TempData["Success"] = "Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            if (movie == null)
                return NotFound(); return View(movie);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            await _movieService.DeleteMovie(id);

            TempData["Success"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}