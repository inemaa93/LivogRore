

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LivogRøre.Data;
using LivogRøre.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LivogRøre.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CompanyController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Company,Admin")]
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
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BecomeCompany()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                if (!await _context.Roles.AnyAsync(r => r.Name == "Company"))
                {
                    await _context.Roles.AddAsync(new IdentityRole("Company"));
                    await _context.SaveChangesAsync();
                }

                if (!await _userManager.IsInRoleAsync(user, "Company"))
                {
                    await _userManager.AddToRoleAsync(user, "Company");
                    
                    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Message"] = "Du er nå registrert som bedrift!";
                }
            }

            return RedirectToAction("UserHome", "Home");
        }

    }
}