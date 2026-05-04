using cinemaSystem.Models;

namespace cinemaSystem.Services
{
    public interface IMovieService
    {
        Task CreateMovie(Movie movie, IFormFile? mainImageFile, List<IFormFile>? subImages, int[] selectedActors);
        Task UpdateMovie(Movie movie, int id, IFormFile? mainImageFile, List<IFormFile>? subImages, int[] selectedActors);
        Task DeleteMovie(int id);
    }
}