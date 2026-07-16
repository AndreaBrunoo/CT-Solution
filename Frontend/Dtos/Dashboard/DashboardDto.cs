using Frontend.Dtos.WorkLog;

namespace Frontend.Dtos.Dashboard;

public class DashboardDto
{
    public int MyHoursWeek { get; set; }
    public int MyWorkLogsCount { get; set; }
    public int ProjectsCount { get; set; }
    public int CompaniesCount { get; set; }
    public List<WorkLogDto>? RecentWorkLogs { get; set; }
}
