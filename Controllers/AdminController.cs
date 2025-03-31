using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LivogRøre.Controllers
{
    [Authorize(Roles = "Admin")]  //Restricts this page to Admin only (Figure out how to make it invis for users)
    public class AdminPageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminPageController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager)); // Prevents null error
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager)); // Prevents null error
        }
        
        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> AssignUserRole(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
                TempData["Message"] = $" User {email} has been assigned to user role";
            }
            else
            {
                TempData["Message"] = $" User {email} was not found";
            }
            
            return RedirectToAction("Users");
        }
    }
}
