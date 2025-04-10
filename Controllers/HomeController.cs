using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LivogRøre.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LivogRøre.Data;
using Microsoft.CodeAnalysis;

namespace LivogRøre.backend.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public HomeController(  //Legger de under hverandre så vi slipper å ha en utrolig lang linje kode x)
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
    public IActionResult UserHome()
    {
        var events = _context.Events
            .OrderByDescending(e => e.Date)
            .ToList();
        return View("~/Views/UserPage/Home.cshtml", events); // Need to spesify path for some reason..
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
