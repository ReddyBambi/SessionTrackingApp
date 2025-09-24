using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SessionTrackingApp.Models;

namespace SessionTrackingApp.Controllers
{
    public class SessionController : Controller
    {
        private readonly IConfiguration _config;

        public SessionController(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = new List<SessionViewModel>();

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM UserSessionTracking ORDER BY UpdatedAt DESC", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sessions.Add(new SessionViewModel
                {
                    SessionId = reader["SessionId"].ToString(),
                    StartTime = Convert.ToDateTime(reader["StartTime"]),
                    LastPage = reader["LastPage"].ToString(),
                    PagesVisited = reader["PagesVisited"].ToString(),
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                });
            }

            return View(sessions);
        }
    }

}
