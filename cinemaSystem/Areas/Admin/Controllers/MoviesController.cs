using cinemaSystem.Data;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

       
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .ToListAsync();

            return View(movies);
        }

        
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
            ViewBag.Actors = await _context.Actors.ToListAsync();

            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Movie movie,
            IFormFile? mainImageFile,
            List<IFormFile>? subImages,
            int[] selectedActors)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
                ViewBag.Actors = await _context.Actors.ToListAsync();
                return View(movie);
            }

            if (mainImageFile != null)
            {
                movie.MainImage = await SaveImage(mainImageFile);
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            if (selectedActors != null)
            {
                foreach (var actorId in selectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }
            }

            if (subImages != null)
            {
                foreach (var file in subImages)
                {
                    if (file.Length > 0)
                    {
                        _context.MovieImages.Add(new MovieImage
                        {
                            MovieId = movie.Id,
                            ImageUrl = await SaveImage(file)
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Movie added successfully!";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
            ViewBag.Actors = await _context.Actors.ToListAsync();

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
            int[] selectedActors)
        {
            var existingMovie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existingMovie == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(movie);
            }

            if (mainImageFile != null && mainImageFile.Length > 0)
            {
                DeleteImage(existingMovie.MainImage);
                existingMovie.MainImage = await SaveImage(mainImageFile);
            }

            existingMovie.Name = movie.Name;
            existingMovie.Price = movie.Price;
            existingMovie.DateTime = movie.DateTime;
            existingMovie.CategoryId = movie.CategoryId;
            existingMovie.CinemaId = movie.CinemaId;

            if (subImages != null && subImages.Count > 0)
            {
                foreach (var file in subImages)
                {
                    if (file.Length > 0)
                    {
                        _context.MovieImages.Add(new MovieImage
                        {
                            MovieId = existingMovie.Id,
                            ImageUrl = await SaveImage(file)
                        });
                    }
                }
            }

            _context.MovieActors.RemoveRange(existingMovie.MovieActors);

            if (selectedActors != null)
            {
                foreach (var actorId in selectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = existingMovie.Id,
                        ActorId = actorId
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Movie updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie != null)
            {
                DeleteImage(movie.MainImage);

                foreach (var img in movie.Images)
                {
                    DeleteImage(img.ImageUrl);
                }
                _context.MovieImages.RemoveRange(movie.Images);
                _context.MovieActors.RemoveRange(movie.MovieActors);
                _context.Movies.Remove(movie);

                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Movie deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

        
        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(_env.WebRootPath, "Images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/Images/" + fileName;
        }

        private void DeleteImage(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            string fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}