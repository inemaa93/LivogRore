using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivogRøre.Controllers
{
    [Authorize(Roles = "Admin")]  //Restricts this page to Admin only
    public class AdminPageController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }
    }
}
