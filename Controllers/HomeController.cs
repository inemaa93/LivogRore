using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LivogRøre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LivogRøre.Data;
using Microsoft.EntityFrameworkCore;

namespace LivogRøre.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (_signInManager.IsSignedIn(User))
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Ensure "User" role exists
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Check if user already has the role
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    // Re-sign in to refresh claims (so the role is recognized immediately)
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
            }

            return RedirectToAction("UserHome");
        }

        return View();
    }

    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> UserHome()
    {
        try
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                _logger.LogWarning("Identity user not found in UserHome");
                return RedirectToAction("Index");
            }

            // Check if user exists in AppUsers table, if not create it
            var user = await _context.AppUsers
                .Include(u => u.PreferredLocation)
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);

            if (user == null)
            {
                _logger.LogInformation("Creating new AppUser for identity user {UserId}", identityUser.Id);
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

            // Get events, filtered by location if user has a preference
            IQueryable<Event> eventsQuery = _context.Events
                .Include(e => e.Location)
                .OrderByDescending(e => e.Date);

            if (user.PreferredLocationId != null)
            {
                eventsQuery = eventsQuery.Where(e => e.LocationId == user.PreferredLocationId);
            }

            var events = await eventsQuery.ToListAsync();
            return View("~/Views/UserPage/Home.cshtml", events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UserHome action");
            return View("~/Views/UserPage/Home.cshtml", new List<Event>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
