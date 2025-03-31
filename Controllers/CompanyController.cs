

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LivogRøre.Data;
using LivogRøre.Models;

namespace LivogRøre.Controllers
{
    [Authorize(Roles = "Company,Admin")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CompanyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.CreatedBy = user?.Email ?? "Unknown";

                _context.Events.Add(model);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Eventet ble opprettet!";
                return RedirectToAction("CreateEvent");
            }

            return View(model);
        }
    }
}