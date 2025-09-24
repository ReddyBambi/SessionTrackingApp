using SessionTrackingApp.Models;
using SessionTrackingApp.Repositories;

namespace SessionTrackingApp.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync()
        {
            return await _sessionRepository.GetAllSessionsAsync();
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(string sessionId)
        {
            return await _sessionRepository.GetSessionByIdAsync(sessionId);
        }
    }
}