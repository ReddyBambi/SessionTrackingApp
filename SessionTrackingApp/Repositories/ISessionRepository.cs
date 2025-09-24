using SessionTrackingApp.Models;

namespace SessionTrackingApp.Repositories
{
    public interface ISessionRepository
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync();
        Task<SessionViewModel?> GetSessionByIdAsync(string sessionId);
        Task SaveSessionAsync(string sessionId, DateTime startTime, string lastPage, string pagesVisited);
        Task UpdateSessionAsync(string sessionId, string lastPage, string pagesVisited);
        Task<bool> SessionExistsAsync(string sessionId);
    }
}