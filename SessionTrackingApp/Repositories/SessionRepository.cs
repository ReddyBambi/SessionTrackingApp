using Microsoft.Data.SqlClient;
using SessionTrackingApp.Models;

namespace SessionTrackingApp.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _connectionString;

        public SessionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not found");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync()
        {
            var sessions = new List<SessionViewModel>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM UserSessionTracking ORDER BY UpdatedAt DESC", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sessions.Add(new SessionViewModel
                {
                    SessionId = reader["SessionId"].ToString() ?? string.Empty,
                    StartTime = Convert.ToDateTime(reader["StartTime"]),
                    LastPage = reader["LastPage"].ToString() ?? string.Empty,
                    PagesVisited = reader["PagesVisited"].ToString() ?? string.Empty,
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                });
            }

            return sessions;
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(string sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT * FROM UserSessionTracking WHERE SessionId = @SessionId", connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new SessionViewModel
                {
                    SessionId = reader["SessionId"].ToString() ?? string.Empty,
                    StartTime = Convert.ToDateTime(reader["StartTime"]),
                    LastPage = reader["LastPage"].ToString() ?? string.Empty,
                    PagesVisited = reader["PagesVisited"].ToString() ?? string.Empty,
                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                };
            }

            return null;
        }

        public async Task<bool> SessionExistsAsync(string sessionId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("SELECT COUNT(1) FROM UserSessionTracking WHERE SessionId = @SessionId", connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);

            var count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task SaveSessionAsync(string sessionId, DateTime startTime, string lastPage, string pagesVisited)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO UserSessionTracking (SessionId, StartTime, LastPage, PagesVisited, UpdatedAt)
                VALUES (@SessionId, @StartTime, @LastPage, @PagesVisited, @UpdatedAt)", connection);

            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@StartTime", startTime);
            command.Parameters.AddWithValue("@LastPage", lastPage);
            command.Parameters.AddWithValue("@PagesVisited", pagesVisited);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateSessionAsync(string sessionId, string lastPage, string pagesVisited)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE UserSessionTracking
                SET LastPage = @LastPage, PagesVisited = @PagesVisited, UpdatedAt = @UpdatedAt
                WHERE SessionId = @SessionId", connection);

            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@LastPage", lastPage);
            command.Parameters.AddWithValue("@PagesVisited", pagesVisited);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }
    }
}