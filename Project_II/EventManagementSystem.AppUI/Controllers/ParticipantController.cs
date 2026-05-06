using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.DataAccessLayer.Interfaces;
using EventManagementSystem.DataAccessLayer.Models;
using EventManagementSystem.AppUI.Filters;
using EventManagementSystem.AppUI.Models;

namespace EventManagementSystem.AppUI.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IParticipantEventRepository _participantEventRepo;
        private readonly ISessionRepository _sessionRepo;

        public ParticipantController(
            IUserRepository userRepo,
            IEventRepository eventRepo,
            IParticipantEventRepository participantEventRepo,
            ISessionRepository sessionRepo)
        {
            _userRepo             = userRepo;
            _eventRepo            = eventRepo;
            _participantEventRepo = participantEventRepo;
            _sessionRepo          = sessionRepo;
        }

        private string? GetLoggedInEmail() => HttpContext.Session.GetString("UserEmail");

        // GET: Participant/Explore
        public async Task<IActionResult> Explore(string? category)
        {
            IEnumerable<EventDetails> events;

            if (string.IsNullOrEmpty(category))
                events = await _eventRepo.GetActiveEventsAsync();
            else
                events = await _eventRepo.GetByCategoryAsync(category);

            ViewBag.Categories  = await _eventRepo.GetCategoriesAsync();
            ViewBag.SelectedCat = category;
            ViewBag.IsLoggedIn  = GetLoggedInEmail() != null;
            return View(events);
        }

        // GET: Participant/Register
        public IActionResult Register()
        {
            if (GetLoggedInEmail() != null)
                return RedirectToAction("Dashboard");
            return View(new RegisterViewModel());
        }

        // POST: Participant/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userInfo = new UserInfo
            {
                EmailId  = model.EmailId,
                UserName = model.UserName,
                Password = model.Password,
                Role     = "Participant"
            };

            var result = await _userRepo.RegisterAsync(userInfo);

            if (result)
            {
                TempData["Success"] = "Registration successful! Please sign in to continue.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("EmailId", "This email is already registered. Please login instead.");
            return View(model);
        }

        // GET: Participant/Login
        public IActionResult Login()
        {
            if (GetLoggedInEmail() != null)
                return RedirectToAction("Dashboard");
            return View(new LoginViewModel());
        }

        // POST: Participant/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepo.LoginAsync(model.Email, model.Password);

            if (user != null && user.Role == "Participant")
            {
                HttpContext.Session.Remove("AdminEmail");
                HttpContext.Session.Remove("AdminName");

                HttpContext.Session.SetString("UserEmail", user.EmailId);
                HttpContext.Session.SetString("UserName",  user.UserName);
                TempData["Success"] = "Welcome back, " + user.UserName + "!";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid email or password. Please try again.");
            return View(model);
        }

        // GET: Participant/Dashboard
        [ParticipantAuth]
        public async Task<IActionResult> Dashboard()
        {
            var userEmail     = GetLoggedInEmail()!;
            var registrations = await _participantEventRepo.GetByParticipantAsync(userEmail);
            ViewBag.UserName  = HttpContext.Session.GetString("UserName");
            return View(registrations);
        }

        // GET: Participant/RegisteredEvents
        [ParticipantAuth]
        public async Task<IActionResult> RegisteredEvents()
        {
            var userEmail     = GetLoggedInEmail()!;
            var registrations = await _participantEventRepo.GetByParticipantAsync(userEmail);
            ViewBag.UserName  = HttpContext.Session.GetString("UserName");
            return View(registrations);
        }

        // GET: Participant/MySessions
        [ParticipantAuth]
        public async Task<IActionResult> MySessions()
        {
            var userEmail     = GetLoggedInEmail()!;
            var registrations = await _participantEventRepo.GetByParticipantAsync(userEmail);

            var eventIds    = registrations.Select(r => r.EventId).Distinct().ToList();
            var allSessions = new List<SessionInfo>();

            foreach (var eventId in eventIds)
            {
                var sessions = await _sessionRepo.GetByEventIdAsync(eventId);
                allSessions.AddRange(sessions);
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(allSessions);
        }

        // POST: Participant/RegisterEvent
        [ParticipantAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEvent(Guid eventId)
        {
            var userEmail = GetLoggedInEmail()!;

            var alreadyRegistered = await _participantEventRepo.IsAlreadyRegisteredAsync(userEmail, eventId);
            if (alreadyRegistered)
            {
                TempData["Error"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Home", new { id = eventId });
            }

            var entry = new ParticipantEventDetails
            {
                ParticipantEmailId = userEmail,
                EventId            = eventId,
                IsAttended         = false
            };

            var result = await _participantEventRepo.RegisterAsync(entry);

            if (result)
                TempData["Success"] = "You have successfully registered for this event!";
            else
                TempData["Error"] = "Registration failed. Please try again.";

            return RedirectToAction("Details", "Home", new { id = eventId });
        }

        // POST: Participant/MarkAttendance
        [ParticipantAuth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(Guid registrationId, bool isAttended)
        {
            var userEmail    = GetLoggedInEmail()!;
            var registration = await _participantEventRepo.GetByIdAsync(registrationId);

            if (registration == null || registration.ParticipantEmailId != userEmail)
            {
                TempData["Error"] = "Registration not found.";
                return RedirectToAction("RegisteredEvents");
            }

            if (registration.Event?.EventDate.Date > DateTime.Today)
            {
                TempData["Error"] = "You cannot mark attendance for a future event.";
                return RedirectToAction("RegisteredEvents");
            }

            var result = await _participantEventRepo.UpdateAttendanceAsync(registrationId, isAttended);

            if (result)
                TempData["Success"] = isAttended ? "Attendance marked successfully!" : "Attendance unmarked.";
            else
                TempData["Error"] = "Failed to update attendance. Please try again.";

            return RedirectToAction("RegisteredEvents");
        }

        // GET: Participant/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserName");
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
    }
}
