using EventManagementSystem.DataAccessLayer.Models;

namespace EventManagementSystem.AppUI.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<EventDetails> Events { get; set; } = [];
        public Dictionary<Guid, List<SessionInfo>> SessionsByEvent { get; set; } = [];
        public int TotalEvents => Events.Count();
        public int ActiveEvents => Events.Count(e => e.Status == "Active");
        public int InactiveEvents => Events.Count(e => e.Status == "In-Active");
        public int TotalSessions => SessionsByEvent.Values.Sum(s => s.Count);
        public int AssignedSessions => SessionsByEvent.Values
            .SelectMany(s => s)
            .Count(s => s.SpeakerId != null);
    }
}
