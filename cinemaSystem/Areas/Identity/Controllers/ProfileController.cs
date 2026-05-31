using cinemaSystem.Models;
using cinemaSystem.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cinemaSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);


            if (user is null) return NotFound();
            //ApplicationUserVM applicationUserVM = new()
            //{


            //    FName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    UserName = user.UserName,
            //    PhoneNumber = user.PhoneNumber,
            //    Address = user.Address,
            //};





            ApplicationUserVM applicationUserVM = user.Adapt<ApplicationUserVM>();

            return View(applicationUserVM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfile(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
                return View("Index", applicationUserVM);

            var user = await _userManager.GetUserAsync(User);


            if (user is null) return NotFound();

            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.Email = applicationUserVM.Email;
            user.UserName = applicationUserVM.UserName;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;

            await _userManager.UpdateAsync(user);

            TempData["success_notification"] = "Update User Information successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}
