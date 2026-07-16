using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Frontend.Dtos.WorkLog;

namespace Frontend.Services;

public class WorkLogService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public WorkLogService(HttpClient http, ILocalStorageService localStorage)
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

    // GET api/WorkLog/mine
    public async Task<List<WorkLogDto>?> GetMineAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<WorkLogDto>>("api/WorkLog/mine");
    }

    // GET api/WorkLog
    public async Task<List<WorkLogDto>?> GetAllAsync()
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<List<WorkLogDto>>("api/WorkLog");
    }

    // GET api/WorkLog/{id}
    public async Task<WorkLogDto?> GetByIdAsync(Guid id)
    {
        await EnsureBearerAsync();
        return await _http.GetFromJsonAsync<WorkLogDto>($"api/WorkLog/{id}");
    }

    // POST api/WorkLog/create
    public async Task<bool> CreateAsync(CreateWorkLogDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsJsonAsync("api/WorkLog/create", dto);
        return res.IsSuccessStatusCode;
    }

    // PUT api/WorkLog/update
    public async Task<bool> UpdateAsync(UpdateWorkLogDto dto)
    {
        await EnsureBearerAsync();
        var res = await _http.PutAsJsonAsync("api/WorkLog/update", dto);
        return res.IsSuccessStatusCode;
    }

    // DELETE api/WorkLog/delete?id=GUID
    public async Task<bool> DeleteAsync(Guid id)
    {
        await EnsureBearerAsync();
        var res = await _http.DeleteAsync($"api/WorkLog/delete?id={id}");
        return res.IsSuccessStatusCode;
    }

    // POST api/WorkLog/{id}/change-status?newStatusId=GUID
    public async Task<bool> ChangeStatusAsync(Guid id, Guid newStatusId)
    {
        await EnsureBearerAsync();
        var res = await _http.PostAsync(
            $"api/WorkLog/{id}/change-status?newStatusId={newStatusId}",
            null
        );

        return res.IsSuccessStatusCode;
    }
}