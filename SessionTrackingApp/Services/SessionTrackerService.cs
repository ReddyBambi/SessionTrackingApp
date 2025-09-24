using Microsoft.Data.SqlClient;

namespace SessionTrackingApp.Services
{
    public class SessionTrackerService
    {
        private readonly string _connectionString;

        public SessionTrackerService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task SaveSessionAsync(ISession session)
        {
            var sessionId = session.GetString("SessionId");
            var startTime = DateTime.Parse(session.GetString("StartTime") ?? DateTime.UtcNow.ToString());
            var pagesVisited = session.GetString("PagesVisited");
            var lastPage = pagesVisited?.Split(";").Reverse().Skip(1).FirstOrDefault();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
            IF EXISTS (SELECT 1 FROM UserSessionTracking WHERE SessionId = @SessionId)
                UPDATE UserSessionTracking
                SET LastPage = @LastPage, PagesVisited = @PagesVisited, UpdatedAt = @UpdatedAt
                WHERE SessionId = @SessionId
            ELSE
                INSERT INTO UserSessionTracking (SessionId, StartTime, LastPage, PagesVisited, UpdatedAt)
                VALUES (@SessionId, @StartTime, @LastPage, @PagesVisited, @UpdatedAt)
        ", conn);

            cmd.Parameters.AddWithValue("@SessionId", sessionId);
            cmd.Parameters.AddWithValue("@StartTime", startTime);
            cmd.Parameters.AddWithValue("@LastPage", lastPage ?? "/");
            cmd.Parameters.AddWithValue("@PagesVisited", pagesVisited ?? "");
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }
    }

}
