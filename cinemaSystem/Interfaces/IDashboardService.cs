using cinemaSystem.Models;

namespace cinemaSystem.Services
{
    public interface IDashboardService
    {
        Task<int> GetMoviesCount();
        Task<int> GetActorsCount();
        Task<int> GetCinemasCount();
        Task<int> GetCategoriesCount();

        Task<List<Actor>> GetRecentActors();
        Task<List<Movie>> GetRecentMovies();
    }
}