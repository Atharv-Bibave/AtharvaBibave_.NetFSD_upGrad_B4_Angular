using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.DataAccessLayer.Interfaces;

namespace EventManagementSystem.AppUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _eventRepository;
        
        public HomeController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IActionResult> Index(string? category)
        {
            IEnumerable<EventManagementSystem.DataAccessLayer.Models.EventDetails> events;
            if (string.IsNullOrEmpty(category))
                events = await _eventRepository.GetActiveEventsAsync();
            else
                events = await _eventRepository.GetByCategoryAsync(category);

            ViewBag.Categories = await _eventRepository.GetCategoriesAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }
            return View(ev);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new AppUI.Models.ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}
