using cinemaSystem.Data;
using cinemaSystem.Interfaces.cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public MovieService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task CreateMovie(Movie movie, IFormFile? mainImageFile, List<IFormFile>? subImages, int[] selectedActors)
        {
            if (mainImageFile != null)
                movie.MainImage = await _imageService.SaveImageAsync(mainImageFile);

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
                    _context.MovieImages.Add(new MovieImage
                    {
                        MovieId = movie.Id,
                        ImageUrl = await _imageService.SaveImageAsync(file)
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateMovie(Movie movie, int id, IFormFile? mainImageFile, List<IFormFile>? subImages, int[] selectedActors)
        {
            var existing = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null) return;

            if (mainImageFile != null)
            {
                _imageService.DeleteImage(existing.MainImage);
                existing.MainImage = await _imageService.SaveImageAsync(mainImageFile);
            }

            existing.Name = movie.Name;
            existing.Price = movie.Price;
            existing.DateTime = movie.DateTime;
            existing.CategoryId = movie.CategoryId;
            existing.CinemaId = movie.CinemaId;

            _context.MovieActors.RemoveRange(existing.MovieActors);

            if (selectedActors != null)
            {
                foreach (var actorId in selectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = existing.Id,
                        ActorId = actorId
                    });
                }
            }

            if (subImages != null)
            {
                foreach (var file in subImages)
                {
                    _context.MovieImages.Add(new MovieImage
                    {
                        MovieId = existing.Id,
                        ImageUrl = await _imageService.SaveImageAsync(file)
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Images)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return;

            _imageService.DeleteImage(movie.MainImage);

            foreach (var img in movie.Images)
                _imageService.DeleteImage(img.ImageUrl);

            _context.MovieImages.RemoveRange(movie.Images);
            _context.MovieActors.RemoveRange(movie.MovieActors);
            _context.Movies.Remove(movie);

            await _context.SaveChangesAsync();
        }
    }
}