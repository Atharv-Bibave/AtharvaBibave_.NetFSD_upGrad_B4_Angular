using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.AppUI.Filters;
using EventManagementSystem.AppUI.Models;

namespace EventManagementSystem.AppUI.Controllers
{
    public class SpeakerController : Controller
    {
        private readonly ISpeakerRepository _repo;

        public SpeakerController(ISpeakerRepository repo)
        {
            _repo = repo;
        }

        // GET: Speaker/Index
        [AdminAuth]
        public async Task<IActionResult> Index()
        {
            var speakers = await _repo.GetAllAsync();
            return View(speakers);
        }

        // GET: Speaker/Create
        [AdminAuth]
        public IActionResult Create()
        {
            return View(new SpeakerViewModel());
        }

        // POST: Speaker/Create
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpeakerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new SpeakersDetails { SpeakerName = model.SpeakerName };
            var result = await _repo.AddAsync(entity);

            if (result)
                TempData["Success"] = "Speaker added successfully.";
            else
                TempData["Error"] = "Could not add speaker. Please try again.";

            return RedirectToAction("Index");
        }

        // GET: Speaker/Delete/{id}
        [AdminAuth]
        public async Task<IActionResult> Delete(Guid id)
        {
            var speaker = await _repo.GetByIdAsync(id);
            if (speaker == null)
            {
                TempData["Error"] = "Speaker not found.";
                return RedirectToAction("Index");
            }
            ViewBag.AssignedSessionCount = await _repo.GetAssignedSessionCountAsync(id);
            return View(speaker);
        }

        // POST: Speaker/Delete
        [AdminAuth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _repo.DeleteAsync(id);

            if (result)
                TempData["Success"] = "Speaker removed successfully.";
            else
                TempData["Error"] = "Failed to remove speaker.";

            return RedirectToAction("Index");
        }
    }
}
