using SessionTrackingApp.Models;

namespace SessionTrackingApp.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync();
        Task<SessionViewModel?> GetSessionByIdAsync(string sessionId);
    }
}