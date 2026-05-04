using cinemaSystem.Models;
using cinemaSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var actors = await _actorService.GetAllAsync(ct);
            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile? imageFile, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(actor);

            await _actorService.CreateAsync(actor, imageFile, ct);

            TempData["Success"] = "Actor added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var actor = await _actorService.GetByIdAsync(id, ct);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? imageFile, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(actor);

            await _actorService.UpdateAsync(id, actor, imageFile, ct);

            TempData["Success"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var actor = await _actorService.GetByIdAsync(id, ct);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            await _actorService.DeleteAsync(id, ct);

            TempData["Success"] = "Actor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}