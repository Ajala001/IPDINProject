namespace App.Core.DTOs.Responses
{
    public class DashboardStatsDto
    {
        public int TrainingsCount { get; set; }
        public int UsersCount { get; set; }
        public int ExamsCount { get; set; }
        public string Services { get; set; } = null!;
    }
}
