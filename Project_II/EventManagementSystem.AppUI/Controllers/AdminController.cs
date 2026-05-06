using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.AppUI.Filters;
using EventManagementSystem.AppUI.Models;

namespace EventManagementSystem.AppUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IParticipantEventRepository _participantEventRepo;
        private readonly ISpeakerRepository _speakerRepository;

        public AdminController(
            IEventRepository eventRepository,
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IParticipantEventRepository participantEventRepo,
            ISpeakerRepository speakerRepository)
        {
            _eventRepository     = eventRepository;
            _userRepository      = userRepository;
            _sessionRepository   = sessionRepository;
            _participantEventRepo = participantEventRepo;
            _speakerRepository   = speakerRepository;
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminEmail") != null)
                return RedirectToAction("Dashboard");
            return View(new LoginViewModel());
        }

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepository.LoginAsync(model.Email, model.Password);

            if (user != null && user.Role == "Admin")
            {
                HttpContext.Session.Remove("UserEmail");
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.SetString("AdminEmail", user.EmailId);
                HttpContext.Session.SetString("AdminName",  user.UserName);
                TempData["Success"] = "Welcome, " + user.UserName + "!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid admin credentials. Please try again.");
            return View(model);
        }

        // GET: Admin/Logout
        [AdminAuth]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminEmail");
            HttpContext.Session.Remove("AdminName");
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }

        // GET: Admin/Dashboard
        [AdminAuth]
        public async Task<IActionResult> Dashboard()
        {
            var events   = (await _eventRepository.GetAllAsync()).ToList();
            var sessions = (await _sessionRepository.GetAllAsync()).ToList();

            var vm = new AdminDashboardViewModel
            {
                Events = events,
                SessionsByEvent = sessions
                    .GroupBy(s => s.EventId)
                    .ToDictionary(g => g.Key, g => g.ToList())
            };

            return View(vm);
        }

        // GET: Admin/Index — Manage Events list
        [AdminAuth]
        public async Task<IActionResult> Index()
        {
            var events = await _eventRepository.GetAllAsync();
            return View(events);
        }

        // GET: Admin/Details/{id} — View a single event with sessions (FIX: was missing)
        [AdminAuth]
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

        // GET: Admin/Create
        [AdminAuth]
        public IActionResult Create()
        {
            return View(new EventViewModel());
        }

        // POST: Admin/Create
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (model.EventDate.Date <= DateTime.Today)
                ModelState.AddModelError("EventDate", "Event date must be a future date.");

            if (!ModelState.IsValid)
                return View(model);

            var entity = new EventDetails
            {
                EventName     = model.EventName,
                EventCategory = model.EventCategory,
                EventDate     = model.EventDate,
                Description   = model.Description,
                Status        = model.Status
            };

            var result = await _eventRepository.AddAsync(entity);
            if (result)
            {
                TempData["Success"] = $"Event \"{model.EventName}\" created successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to save event. Please try again.");
            return View(model);
        }

        // GET: Admin/Edit/{id}
        [AdminAuth]
        public async Task<IActionResult> Edit(Guid id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            var model = new EventViewModel
            {
                EventId       = ev.EventId,
                EventName     = ev.EventName,
                EventCategory = ev.EventCategory,
                EventDate     = ev.EventDate,
                Description   = ev.Description,
                Status        = ev.Status,
                IsEditMode    = true
            };
            return View(model);
        }

        // POST: Admin/Edit
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventViewModel model)
        {
            if (model.EventDate.Date <= DateTime.Today)
                ModelState.AddModelError("EventDate", "Event date must be a future date.");

            if (!ModelState.IsValid)
                return View(model);

            var entity = new EventDetails
            {
                EventId       = model.EventId,
                EventName     = model.EventName,
                EventCategory = model.EventCategory,
                EventDate     = model.EventDate,
                Description   = model.Description,
                Status        = model.Status
            };

            var result = await _eventRepository.UpdateAsync(entity);
            if (result)
            {
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update event. Please try again.");
            return View(model);
        }

        // GET: Admin/Delete/{id}
        [AdminAuth]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }
            return View(ev);
        }

        // POST: Admin/Delete
        [AdminAuth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _eventRepository.DeleteAsync(id);

            if (result)
                TempData["Success"] = "Event deleted successfully.";
            else
                TempData["Error"] = "Could not delete the event. Please try again.";

            return RedirectToAction("Index");
        }

        // POST: Admin/ToggleStatus/{id}
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index");
            }

            ev.Status = ev.Status == "Active" ? "In-Active" : "Active";
            var result = await _eventRepository.UpdateAsync(ev);

            if (result)
                TempData["Success"] = $"Status changed to {ev.Status}.";
            else
                TempData["Error"] = "Failed to update event status.";

            return RedirectToAction("Index");
        }

        // GET: Admin/Participants
        [AdminAuth]
        public async Task<IActionResult> Participants(Guid? eventId)
        {
            var registrations = await _participantEventRepo.GetAllAsync();

            if (eventId.HasValue)
            {
                registrations = registrations.Where(r => r.EventId == eventId.Value);
                var ev = await _eventRepository.GetByIdAsync(eventId.Value);
                ViewBag.FilterEventName = ev?.EventName ?? "Selected Event";
                ViewBag.FilterEventId   = eventId;
            }

            ViewBag.Events = await _eventRepository.GetAllAsync();
            return View(registrations);
        }

        // POST: Admin/UpdateAttendance
        [AdminAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttendance(Guid id, bool isAttended, Guid? filterEventId)
        {
            var result = await _participantEventRepo.UpdateAttendanceAsync(id, isAttended);

            if (result)
                TempData["Success"] = "Attendance updated.";
            else
                TempData["Error"] = "Failed to update attendance.";

            return RedirectToAction("Participants", new { eventId = filterEventId });
        }

        // GET: Admin/SpeakerAssignments
        [AdminAuth]
        public async Task<IActionResult> SpeakerAssignments()
        {
            var speakers = (await _speakerRepository.GetAllAsync()).ToList();
            var sessions = (await _sessionRepository.GetAllAsync()).ToList();

            var assignments = speakers.ToDictionary(
                sp => sp,
                sp => sessions.Where(s => s.SpeakerId == sp.SpeakerId).ToList()
            );

            ViewBag.UnassignedSessions = sessions.Where(s => s.SpeakerId == null).ToList();
            return View(assignments);
        }
    }
}
