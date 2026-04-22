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

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
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
            
            if (mainImageFile != null && mainImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid() + Path.GetExtension(mainImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mainImageFile.CopyToAsync(stream);
                }

                movie.MainImage = "/images/" + fileName;
            }

           
            if (subImages != null && subImages.Count > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                List<string> imagesPaths = new List<string>();

                foreach (var file in subImages)
                {
                    if (file.Length > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        imagesPaths.Add("/images/" + fileName);
                    }
                }

                movie.SubImages = imagesPaths;
            }

            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                
                if (selectedActors != null && selectedActors.Length > 0)
                {
                    foreach (var actorId in selectedActors)
                    {
                        _context.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
            ViewBag.Actors = await _context.Actors.ToListAsync();

            return View(movie);
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
            ViewBag.Actors = await _context.Actors.ToListAsync();

            ViewBag.SelectedActors = movie.MovieActors.Select(ma => ma.ActorId).ToArray();

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
            if (id != movie.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingMovie = await _context.Movies
                    .Include(m => m.MovieActors)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (existingMovie == null)
                    return NotFound();

                
                if (mainImageFile != null && mainImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string fileName = Guid.NewGuid() + Path.GetExtension(mainImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await mainImageFile.CopyToAsync(stream);
                    }

                    existingMovie.MainImage = "/images/" + fileName;
                }

                
                existingMovie.Name = movie.Name;
                existingMovie.Price = movie.Price;
                existingMovie.DateTime = movie.DateTime;
                existingMovie.CategoryId = movie.CategoryId;
                existingMovie.CinemaId = movie.CinemaId;
                existingMovie.SubImages = movie.SubImages;

                
                var oldActors = _context.MovieActors.Where(ma => ma.MovieId == id);
                _context.MovieActors.RemoveRange(oldActors);
                if (selectedActors != null && selectedActors.Length > 0)
                {
                    foreach (var actorId in selectedActors)
                    {
                        _context.MovieActors.Add(new MovieActor
                        {
                            MovieId = id,
                            ActorId = actorId
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();
            ViewBag.Actors = await _context.Actors.ToListAsync();

            return View(movie);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie != null)
            {
                var movieActors = _context.MovieActors.Where(ma => ma.MovieId == id);
                _context.MovieActors.RemoveRange(movieActors);

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}