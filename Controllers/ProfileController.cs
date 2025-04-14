using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LivogRøre.Data;
using LivogRøre.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LivogRøre.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        if (identityUser == null) return NotFound();

        var user = await _context.AppUsers
            .Include(u => u.PreferredLocation)
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);

        if (user == null)
        {
            // Create new user profile if it doesn't exist
            user = new User
            {
                IdentityUserId = identityUser.Id,
                Username = identityUser.UserName ?? string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty
            };
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        // Get all locations
        var locations = await _context.Locations
            .OrderBy(l => l.City)
            .ThenBy(l => l.Name)
            .Select(l => new LocationViewModel
            {
                Id = l.Id,
                DisplayText = $"{l.Name}, {l.City}"
            })
            .ToListAsync();

        ViewBag.Locations = new SelectList(locations, "Id", "DisplayText");

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(User model)
    {
        if (!ModelState.IsValid)
        {
            // Re-populate the locations list if validation fails
            var locations = await _context.Locations
                .OrderBy(l => l.City)
                .ThenBy(l => l.Name)
                .Select(l => new LocationViewModel
                {
                    Id = l.Id,
                    DisplayText = $"{l.Name}, {l.City}"
                })
                .ToListAsync();
            ViewBag.Locations = new SelectList(locations, "Id", "DisplayText");
            return View("Index", model);
        }

        var user = await _context.AppUsers.FindAsync(model.Id);
        if (user == null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.DateOfBirth = model.DateOfBirth;
        user.PreferredLocationId = model.PreferredLocationId;

        await _context.SaveChangesAsync();
        TempData["Message"] = "Profilen din har blitt oppdatert!";
        return RedirectToAction(nameof(Index));
    }
}

public class LocationViewModel
{
    public int Id { get; set; }
    public string DisplayText { get; set; } = string.Empty;
} 