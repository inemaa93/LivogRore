

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LivogRøre.Controllers
{
    [Authorize(Roles = "Company,Admin")]
    public class CompanyController : Controller
    {
        public IActionResult CreateEvent()
        {
            return View();
        }
    }
}