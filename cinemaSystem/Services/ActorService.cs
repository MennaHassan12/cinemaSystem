using cinemaSystem.Data;
using cinemaSystem.Interfaces.cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace cinemaSystem.Services
{
    public class ActorService : IActorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public ActorService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<List<Actor>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Actors.ToListAsync(ct);
        }

        public async Task<Actor?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Actors.FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task CreateAsync(Actor actor, IFormFile? imageFile, CancellationToken ct)
        {
            if (imageFile != null)
            {
                actor.ImageUrl = await _imageService.SaveImageAsync(imageFile);
            }

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(int id, Actor actor, IFormFile? imageFile, CancellationToken ct)
        {
            var existing = await _context.Actors.FirstOrDefaultAsync(a => a.Id == id, ct);

            if (existing == null)
                return;

            existing.Name = actor.Name;

            if (imageFile != null)
            {
                _imageService.DeleteImage(existing.ImageUrl);
                existing.ImageUrl = await _imageService.SaveImageAsync(imageFile);
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Id == id, ct);

            if (actor == null)
                return;

            _imageService.DeleteImage(actor.ImageUrl);

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync(ct);
        }
    }
}