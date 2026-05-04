using cinemaSystem.Models;

namespace cinemaSystem.Services
{
    public interface IActorService
    {
        Task<List<Actor>> GetAllAsync(CancellationToken ct);
        Task<Actor?> GetByIdAsync(int id, CancellationToken ct);
        Task CreateAsync(Actor actor, IFormFile? imageFile, CancellationToken ct);
        Task UpdateAsync(int id, Actor actor, IFormFile? imageFile, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);
    }
}