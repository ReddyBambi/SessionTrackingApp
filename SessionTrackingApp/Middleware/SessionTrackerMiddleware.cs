using SessionTrackingApp.Services;

namespace SessionTrackingApp.Middleware
{
    public class SessionTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionTrackerMiddleware> _logger;

        public SessionTrackerMiddleware(RequestDelegate next, ILogger<SessionTrackerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var session = context.Session;
            var trackerService = context.RequestServices.GetRequiredService<SessionTrackerService>();

            if (string.IsNullOrEmpty(session.GetString("SessionId")))
            {
                session.SetString("SessionId", Guid.NewGuid().ToString());
                session.SetString("StartTime", DateTime.UtcNow.ToString());
            }

            var pages = session.GetString("PagesVisited") ?? "";
            pages += $"{context.Request.Path};";
            session.SetString("PagesVisited", pages);
            await trackerService.SaveSessionAsync(context.Session);
            // You could log this info to a database here
            _logger.LogInformation($"Session: {session.GetString("SessionId")}, Page Visited: {context.Request.Path}");

            await _next(context);
        }
    }

}
