using cinemaSystem.Data;
using cinemaSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetMoviesCount() => await _context.Movies.CountAsync();

        public async Task<int> GetActorsCount() => await _context.Actors.CountAsync();

        public async Task<int> GetCinemasCount() => await _context.Cinemas.CountAsync();

        public async Task<int> GetCategoriesCount() => await _context.Categories.CountAsync();

        public async Task<List<Actor>> GetRecentActors()
        {
            return await _context.Actors
                .OrderByDescending(a => a.Id)
                .Take(6)
                .ToListAsync();
        }

        public async Task<List<Movie>> GetRecentMovies()
        {
            return await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .OrderByDescending(m => m.Id)
                .Take(6)
                .ToListAsync();
        }
    }
}