using SessionTrackingApp.Repositories;

namespace SessionTrackingApp.Services
{
    public class SessionTrackerService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionTrackerService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task SaveSessionAsync(ISession session)
        {
            var sessionId = session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
                return;

            var startTimeString = session.GetString("StartTime");
            var startTime = DateTime.TryParse(startTimeString, out var parsedStartTime)
                ? parsedStartTime
                : DateTime.UtcNow;

            var pagesVisited = session.GetString("PagesVisited") ?? string.Empty;
            var lastPage = GetLastPageFromVisited(pagesVisited);

            var sessionExists = await _sessionRepository.SessionExistsAsync(sessionId);

            if (sessionExists)
            {
                await _sessionRepository.UpdateSessionAsync(sessionId, lastPage, pagesVisited);
            }
            else
            {
                await _sessionRepository.SaveSessionAsync(sessionId, startTime, lastPage, pagesVisited);
            }
        }

        private static string GetLastPageFromVisited(string pagesVisited)
        {
            if (string.IsNullOrEmpty(pagesVisited))
                return "/";

            var pages = pagesVisited.Split(';', StringSplitOptions.RemoveEmptyEntries);
            return pages.LastOrDefault() ?? "/";
        }
    }

}
