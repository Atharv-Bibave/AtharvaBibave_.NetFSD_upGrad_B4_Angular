using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.AppUI.Filters;
using EventManagementSystem.AppUI.Models;

namespace EventManagementSystem.AppUI.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionRepository _repo;
        private readonly IEventRepository _eventRepo;
        private readonly ISpeakerRepository _speakerRepo;

        public SessionController(
            ISessionRepository repo,
            IEventRepository eventRepo,
            ISpeakerRepository speakerRepo)
        {
            _repo        = repo;
            _eventRepo   = eventRepo;
            _speakerRepo = speakerRepo;
        }

        // GET: Session/Index — admin list, optionally filtered by event
        [AdminAuth]
        public async Task<IActionResult> Index(Guid? eventId)
        {
            IEnumerable<SessionInfo> sessions;

            if (eventId.HasValue)
            {
                sessions = await _repo.GetByEventIdAsync(eventId.Value);
                var ev = await _eventRepo.GetByIdAsync(eventId.Value);
                ViewBag.FilterEventName = ev?.EventName ?? "Selected Event";
                ViewBag.FilterEventId   = eventId;
            }
            else
            {
                sessions = await _repo.GetAllAsync();
            }

            return View(sessions);
        }

        // GET: Session/Create
        [AdminAuth]
        public async Task<IActionResult> Create()
        {
            ViewBag.Events   = await _eventRepo.GetAllAsync();
            ViewBag.Speakers = await _speakerRepo.GetAllAsync();
            return View(new SessionViewModel());
        }

        // POST: Session/Create
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionViewModel model)
        {
            if (model.SessionStart >= model.SessionEnd)
                ModelState.AddModelError("", "Session Start time must be before Session End time.");

            if (model.EventId == Guid.Empty)
                ModelState.AddModelError("EventId", "Please select an event.");

            if (!ModelState.IsValid)
            {
                ViewBag.Events   = await _eventRepo.GetAllAsync();
                ViewBag.Speakers = await _speakerRepo.GetAllAsync();
                return View(model);
            }

            var entity = new SessionInfo
            {
                SessionTitle = model.SessionTitle,
                Description  = model.Description,
                SessionUrl   = model.SessionUrl,
                EventId      = model.EventId,
                SpeakerId    = (model.SpeakerId == Guid.Empty) ? null : model.SpeakerId,
                SessionStart = model.SessionStart,
                SessionEnd   = model.SessionEnd
            };

            var result = await _repo.AddAsync(entity);
            if (result)
            {
                TempData["Success"] = $"Session \"{model.SessionTitle}\" created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to save session. Please try again.");
            ViewBag.Events   = await _eventRepo.GetAllAsync();
            ViewBag.Speakers = await _speakerRepo.GetAllAsync();
            return View(model);
        }

        // GET: Session/Edit/{id}
        [AdminAuth]
        public async Task<IActionResult> Edit(Guid id)
        {
            var session = await _repo.GetByIdAsync(id);
            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index");
            }

            var model = new SessionViewModel
            {
                SessionId    = session.SessionId,
                SessionTitle = session.SessionTitle,
                Description  = session.Description,
                SessionUrl   = session.SessionUrl,
                EventId      = session.EventId,
                SpeakerId    = session.SpeakerId,
                SessionStart = session.SessionStart,
                SessionEnd   = session.SessionEnd
            };

            ViewBag.Events   = await _eventRepo.GetAllAsync();
            ViewBag.Speakers = await _speakerRepo.GetAllAsync();
            return View(model);
        }

        // POST: Session/Edit
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SessionViewModel model)
        {
            if (model.SessionStart >= model.SessionEnd)
                ModelState.AddModelError("", "Session Start time must be before Session End time.");

            if (model.EventId == Guid.Empty)
                ModelState.AddModelError("EventId", "Please select an event.");

            if (!ModelState.IsValid)
            {
                ViewBag.Events   = await _eventRepo.GetAllAsync();
                ViewBag.Speakers = await _speakerRepo.GetAllAsync();
                return View(model);
            }

            var entity = new SessionInfo
            {
                SessionId    = model.SessionId,
                SessionTitle = model.SessionTitle,
                Description  = model.Description,
                SessionUrl   = model.SessionUrl,
                EventId      = model.EventId,
                SpeakerId    = (model.SpeakerId == Guid.Empty) ? null : model.SpeakerId,
                SessionStart = model.SessionStart,
                SessionEnd   = model.SessionEnd
            };

            var result = await _repo.UpdateAsync(entity);
            if (result)
            {
                TempData["Success"] = "Session updated successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update session. Please try again.");
            ViewBag.Events   = await _eventRepo.GetAllAsync();
            ViewBag.Speakers = await _speakerRepo.GetAllAsync();
            return View(model);
        }

        // GET: Session/Delete/{id}
        [AdminAuth]
        public async Task<IActionResult> Delete(Guid id)
        {
            var session = await _repo.GetByIdAsync(id);
            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // POST: Session/Delete
        [AdminAuth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _repo.DeleteAsync(id);

            if (result)
                TempData["Success"] = "Session deleted successfully.";
            else
                TempData["Error"] = "Could not delete the session.";

            return RedirectToAction("Index");
        }

        // GET: Session/AssignSpeaker/{id}
        [AdminAuth]
        public async Task<IActionResult> AssignSpeaker(Guid id)
        {
            var session = await _repo.GetByIdAsync(id);
            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index");
            }

            ViewBag.Speakers = await _speakerRepo.GetAllAsync();
            return View(session);
        }

        // POST: Session/AssignSpeaker
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSpeaker(Guid sessionId, Guid? speakerId)
        {
            var session = await _repo.GetByIdAsync(sessionId);
            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index");
            }

            session.SpeakerId = (speakerId == Guid.Empty) ? null : speakerId;
            var result = await _repo.UpdateAsync(session);

            if (result)
                TempData["Success"] = "Speaker assigned successfully!";
            else
                TempData["Error"] = "Failed to assign speaker.";

            return RedirectToAction("Index");
        }

        // GET: Session/ViewSessions/{eventId} — public, no login required
        public async Task<IActionResult> ViewSessions(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                TempData["Error"] = "Invalid event.";
                return RedirectToAction("Index", "Home");
            }

            var ev = await _eventRepo.GetByIdAsync(eventId);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index", "Home");
            }

            var sessions = await _repo.GetByEventIdAsync(eventId);
            ViewBag.EventName = ev.EventName;
            ViewBag.EventId   = eventId;   

            return View(sessions);
        }

        // GET: Session/Details/{id} — public, no login required
        public async Task<IActionResult> Details(Guid id)
        {
            var session = await _repo.GetByIdAsync(id);
            if (session == null)
            {
                TempData["Error"] = "Session not found.";
                return RedirectToAction("Index", "Home");
            }
            return View(session);
        }
    }
}
