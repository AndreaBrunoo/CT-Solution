using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.Status;

namespace Frontend.Services;

public class StatusService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public StatusService(HttpClient http, ILocalStorageService localStorage)
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

    public async Task<StatusDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<StatusDto>($"api/Status/{id}");
    }

    public async Task<List<StatusDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<StatusDto>>("api/Status");
    }

    public async Task<bool> CreateAsync(CreateStatusDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/Status/create", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateStatusDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PutAsJsonAsync("api/Status/update", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/Status/delete?id={id}");
        return res.IsSuccessStatusCode;
    }
}