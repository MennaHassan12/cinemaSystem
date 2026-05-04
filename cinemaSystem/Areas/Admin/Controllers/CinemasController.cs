using cinemaSystem.Interfaces;
using cinemaSystem.Interfaces.cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemasController : Controller
    {
        private readonly IRepository<Cinema> _repo;
        private readonly IImageService _imageService;

        public CinemasController(IRepository<Cinema> repo, IImageService imageService)
        {
            _repo = repo;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var cinemas = await _repo.GetAsync(ct);
            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile? logoFile, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            if (logoFile != null)
                cinema.ImageUrl = await _imageService.SaveImageAsync(logoFile);

            await _repo.CreateAsync(cinema, ct);
            await _repo.CommitAsync(ct);

            TempData["Success"] = "Cinema added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var cinema = await _repo.GetOneAsync(id, ct);

            if (cinema == null)
                return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema, IFormFile? logoFile, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            var existing = await _repo.GetOneAsync(id, ct);

            if (existing == null)
                return NotFound();

            existing.Name = cinema.Name;

            if (logoFile != null)
            {
                _imageService.DeleteImage(existing.ImageUrl);
                existing.ImageUrl = await _imageService.SaveImageAsync(logoFile);
            }

            _repo.Update(existing);
            await _repo.CommitAsync(ct);

            TempData["Success"] = "Cinema updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var cinema = await _repo.GetOneAsync(id, ct);

            if (cinema == null)
                return NotFound();

            return View(cinema);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var cinema = await _repo.GetOneAsync(id, ct);

            if (cinema != null)
            {
                _imageService.DeleteImage(cinema.ImageUrl);

                _repo.Delete(cinema);
                await _repo.CommitAsync(ct);
            }

            TempData["Success"] = "Cinema deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}