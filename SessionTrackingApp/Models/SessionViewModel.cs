namespace SessionTrackingApp.Models
{
    public class SessionViewModel
    {
        public string SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public string LastPage { get; set; }
        public string PagesVisited { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
