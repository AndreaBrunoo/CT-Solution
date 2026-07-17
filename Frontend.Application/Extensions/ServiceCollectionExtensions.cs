using Frontend.Domain.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Frontend.Application.Services;

namespace Frontend.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        return services;
    }
}