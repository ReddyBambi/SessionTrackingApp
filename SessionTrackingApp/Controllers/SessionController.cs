using Microsoft.AspNetCore.Mvc;
using SessionTrackingApp.Services;

namespace SessionTrackingApp.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            return View(sessions);
        }
    }

}
