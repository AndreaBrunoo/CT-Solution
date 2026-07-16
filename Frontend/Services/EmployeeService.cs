using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Employee;

namespace Frontend.Services;

public class EmployeeService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public EmployeeService(HttpClient http, ILocalStorageService localStorage)
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
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<EmployeeDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<EmployeeDto>>("api/Employee");
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<EmployeeDto>($"api/Employee/{id}");
    }

    public async Task<EmployeeDto?> GetProfileAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<EmployeeDto>("api/Employee/me");
    }

    public async Task<List<EmployeeDto>?> GetByProjectAsync(Guid projectId)
    {
        await EnsureBearerAsync();

        return await _http.GetFromJsonAsync<List<EmployeeDto>>(
            $"api/Employee/by-project/{projectId}"
        );
    }
    
    public async Task<bool> UpdateAsync(UpdateEmployeeDto dto)
    {
        await EnsureBearerAsync();

        var res = await _http.PutAsJsonAsync("api/Employee/update", dto);

        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();

        var res = await _http.DeleteAsync($"api/Employee/delete?id={id}");

        return res.IsSuccessStatusCode;
    }
}