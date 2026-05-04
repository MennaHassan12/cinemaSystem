using cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _repo;

        public CategoriesController(IRepository<Category> repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var categories = await _repo.GetAsync(ct);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(category);

            await _repo.CreateAsync(category, ct);
            await _repo.CommitAsync(ct);

            TempData["Success"] = "Category added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var category = await _repo.GetOneAsync(id, ct);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var category = await _repo.GetOneAsync(id, ct);

            if (category != null)
            {
                _repo.Delete(category);
                await _repo.CommitAsync(ct);
            }

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}