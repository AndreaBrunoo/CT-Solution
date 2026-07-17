using Frontend.Domain.Contracts;
using Frontend.Domain.Dtos.WorkLog;
using Frontend.Domain.Dtos.Project;
using Frontend.Domain.Dtos.Company;
using Frontend.Domain.Dtos.Dashboard;
using Frontend.Infrastructure.Api;

namespace Frontend.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IApiClient _api;

    public DashboardService(IApiClient api)
    {
        _api = api;
    }

    public async Task<DashboardDto?> GetKpiAsync()
    {
        // 1️⃣ Tentativo: endpoint dedicato
        try
        {
            var fromApi = await _api.GetAsync<DashboardDto>("api/dashboard/kpi");
            if (fromApi != null)
                return fromApi;
        }
        catch
        {
            // ignora e passa al fallback
        }

        // 2️⃣ Fallback: costruzione manuale
        var dashboard = new DashboardDto();

        try
        {
            var mine = await _api.GetAsync<List<WorkLogDto>>("api/WorkLog/mine") 
                       ?? new List<WorkLogDto>();

            var projects = await _api.GetAsync<List<ProjectDto>>("api/Project") 
                          ?? new List<ProjectDto>();

            var companies = await _api.GetAsync<List<CompanyDto>>("api/Company") 
                           ?? new List<CompanyDto>();

            dashboard.MyWorkLogsCount = mine.Count;

            // Somma ore ultimi 7 giorni
            try
            {
                var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
                dashboard.MyHoursWeek = mine
                    .Where(w => w.Date >= cutoff)
                    .Sum(w => w.HoursCounter);
            }
            catch
            {
                dashboard.MyHoursWeek = mine.Sum(w => w.HoursCounter);
            }

            dashboard.ProjectsCount = projects.Count;
            dashboard.CompaniesCount = companies.Count;

            dashboard.RecentWorkLogs = mine
                .OrderByDescending(w => w.CreatedAt)
                .Take(5)
                .ToList();
        }
        catch
        {
            // In caso di errore totale → dashboard vuoto
            dashboard.MyHoursWeek = 0;
            dashboard.MyWorkLogsCount = 0;
            dashboard.ProjectsCount = 0;
            dashboard.CompaniesCount = 0;
            dashboard.RecentWorkLogs = new List<WorkLogDto>();
        }

        return dashboard;
    }
}