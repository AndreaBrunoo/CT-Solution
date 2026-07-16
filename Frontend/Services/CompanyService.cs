using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Company;

namespace Frontend.Services;

public class CompanyService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public CompanyService(HttpClient http, ILocalStorageService localStorage)
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

    public async Task<CompanyDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<CompanyDto>($"api/Company/{id}");
    }

    public async Task<List<CompanyDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<CompanyDto>>("api/Company");
    }

    public async Task<bool> CreateAsync(CreateCompanyDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/Company/create", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateCompanyDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PutAsJsonAsync("api/Company/update", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/Company/delete?id={id}");
        return res.IsSuccessStatusCode;
    }
}