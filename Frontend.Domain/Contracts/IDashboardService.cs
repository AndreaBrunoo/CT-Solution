using Frontend.Domain.Dtos.Dashboard;

namespace Frontend.Domain.Contracts;

public interface IDashboardService
{
    Task<DashboardDto?> GetKpiAsync();
}