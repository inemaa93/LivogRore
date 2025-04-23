using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LivogRøre.Controllers
{
    [Authorize(Roles = "Admin")]  //Kun Admin rollen kan se
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> CreateUserRole()
        {
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
                return Content("'User' role has been created");
            }
            
            return Content("'User' role already exists");
        }

        public async Task<IActionResult> AssignUserRole(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && !await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
                return Content($"User {user.Email} has is now assigned to 'user' role");
            }
            
            return Content("User not found");
        }
        
    }
}