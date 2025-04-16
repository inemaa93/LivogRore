using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LivogRøre.Models;
using LivogRøre.Data;
using LivogRøre.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LivogRøre.Controllers;

public class EventController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public EventController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(int? locationId)
    {
        // Get selected location from cookie if not provided
        if (!locationId.HasValue)
        {
            var selectedLocationId = Request.Cookies["SelectedLocationId"];
            if (!string.IsNullOrEmpty(selectedLocationId) && int.TryParse(selectedLocationId, out int cookieLocationId))
            {
                locationId = cookieLocationId;
            }
        }

        // Get available locations for the city selector
        ViewBag.AvailableLocations = await _context.Locations.OrderBy(l => l.City).ThenBy(l => l.Name).ToListAsync();
        ViewBag.SelectedLocationId = locationId;

        var query = _context.Events
            .Include(e => e.Location)
            .Include(e => e.User)
            .AsQueryable();

        if (locationId.HasValue)
        {
            query = query.Where(e => e.LocationId == locationId);
        }

        var events = await query.OrderByDescending(e => e.Date).ToListAsync();
        return View(events);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        var model = new CreateEventViewModel
        {
            Locations = await _context.Locations
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = $"{l.Name} {l.City}"
                })
                .ToListAsync()
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        if (ModelState.IsValid)
        {
            var eventEntity = new Event
            {
                Title = model.Title,
                Date = model.Date,
                Description = model.Description,
                LocationId = model.LocationId,
                CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "events");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                eventEntity.ImagePath = $"/images/events/{uniqueFileName}";
            }

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        model.Locations = await _context.Locations
            .Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.Name} {l.City}"
            })
            .ToListAsync();

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> MyEvents(int? locationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var eventsQuery = _context.Events
            .Include(e => e.Location)
            .Where(e => e.CreatedBy == userId)
            .AsQueryable();

        if (locationId.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.LocationId == locationId);
        }

        var events = await eventsQuery.OrderByDescending(e => e.Date).ToListAsync();
        ViewBag.AvailableLocations = await _context.Locations.OrderBy(l => l.City).ThenBy(l => l.Name).ToListAsync();
        ViewBag.SelectedLocationId = locationId;

        return View(events);
    }
}