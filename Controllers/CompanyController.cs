using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LivogRøre.Data;
using LivogRøre.Models;
using LivogRøre.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LivogRøre.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<CompanyController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [Authorize(Roles = "Company,Admin")]
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found when trying to access CreateEvent");
                return NotFound();
            }

            var locations = await _context.Locations.OrderBy(l => l.County).ThenBy(l => l.Name).ToListAsync();

            var model = new CreateEventViewModel
            {
                Locations = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.Name} ({l.County})"
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Company,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var locations = await _context.Locations.OrderBy(l => l.County).ThenBy(l => l.Name).ToListAsync();
                model.Locations = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.Name} ({l.County})"
                }).ToList();

                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found when trying to create event");
                    return NotFound();
                }

                var newEvent = new Event
                {
                    Title = model.Title,
                    Date = model.Date,
                    Description = model.Description,
                    LocationId = model.LocationId,
                    CreatedBy = user.Email ?? "Unknown"
                };

                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    newEvent.ImagePath = "/images/" + uniqueFileName;
                }

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Arrangementet ble opprettet!";
                return RedirectToAction("UserHome", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");

                var locations = await _context.Locations.OrderBy(l => l.County).ThenBy(l => l.Name).ToListAsync();
                model.Locations = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.Name} ({l.County})"
                }).ToList();

                ModelState.AddModelError("", "Det oppstod en feil ved opprettelse av arrangementet. Vennligst prøv igjen.");
                return View(model);
            }
        }
        
        // GET: EditEvent
        [Authorize(Roles = "Company,Admin")]
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (evt == null)
                return NotFound();

            if (User.IsInRole("Admin") || evt.CreatedBy == user?.Email)
                return View(evt);

            return Forbid();
        }

// POST: EditEvent
        [Authorize(Roles = "Company,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(Event model)
        {
            var user = await _userManager.GetUserAsync(User);
            var existing = await _context.Events.FindAsync(model.Id);

            if (existing == null)
                return NotFound();

            if (!User.IsInRole("Admin") && existing.CreatedBy != user?.Email)
                return Forbid();

            if (ModelState.IsValid)
            {
                existing.Title = model.Title;
                existing.Date = model.Date;
                existing.Description = model.Description;
                await _context.SaveChangesAsync();

                TempData["Message"] = "Eventet ble oppdatert!";
                return RedirectToAction("UserHome", "Home");
            }

            return View(model);
        }

// GET: DeleteEvent
        [Authorize(Roles = "Company,Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (evt == null)
                return NotFound();

            if (!User.IsInRole("Admin") && evt.CreatedBy != user?.Email)
                return Forbid();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Eventet ble slettet.";
            return RedirectToAction("UserHome", "Home");
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
