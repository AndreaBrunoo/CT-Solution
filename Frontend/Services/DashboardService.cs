using System.Net.Http.Json;
using Blazored.LocalStorage;
using Frontend.Dtos.Dashboard;
using Frontend.Dtos.WorkLog;
using Frontend.Dtos.Project;
using Frontend.Dtos.Company;

namespace Frontend.Services;

public class DashboardService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public DashboardService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    private async Task EnsureBearerAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<DashboardDto?> GetKpiAsync()
    {
        await EnsureBearerAsync();

        // Try dedicated endpoint first
        try
        {
            var fromApi = await _http.GetFromJsonAsync<DashboardDto>("api/dashboard/kpi");
            if (fromApi != null)
                return fromApi;
        }
        catch { /* ignore and fallback */ }

        // Fallback: compose from available endpoints
        var dashboard = new DashboardDto();

        try
        {
            var mine = await _http.GetFromJsonAsync<List<WorkLogDto>>("api/WorkLog/mine");
            var projects = await _http.GetFromJsonAsync<List<ProjectDto>>("api/Project");
            var companies = await _http.GetFromJsonAsync<List<CompanyDto>>("api/Company");

            mine ??= new List<WorkLogDto>();
            projects ??= new List<ProjectDto>();
            companies ??= new List<CompanyDto>();

            dashboard.MyWorkLogsCount = mine.Count;

            // Sum hours in last 7 days
            try
            {
                var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
                dashboard.MyHoursWeek = mine.Where(w => w.Date >= cutoff).Sum(w => w.HoursCounter);
            }
            catch
            {
                dashboard.MyHoursWeek = mine.Sum(w => w.HoursCounter);
            }

            dashboard.ProjectsCount = projects.Count;
            dashboard.CompaniesCount = companies.Count;

            dashboard.RecentWorkLogs = mine.OrderByDescending(w => w.CreatedAt).Take(5).ToList();
        }
        catch
        {
            // On error return empty dashboard
            dashboard.MyHoursWeek = 0;
            dashboard.MyWorkLogsCount = 0;
            dashboard.ProjectsCount = 0;
            dashboard.CompaniesCount = 0;
            dashboard.RecentWorkLogs = new List<WorkLogDto>();
        }

        return dashboard;
    }
}