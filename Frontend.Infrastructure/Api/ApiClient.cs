using System.Net.Http.Json;

namespace Frontend.Infrastructure.Api;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public Task<T?> GetAsync<T>(string url)
        => _http.GetFromJsonAsync<T>(url);

    public async Task<T?> PostAsync<T>(string url, object body)
    {
        var res = await _http.PostAsJsonAsync(url, body);
        return await res.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string url, object body)
    {
        var res = await _http.PutAsJsonAsync(url, body);
        return await res.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> DeleteAsync(string url)
    {
        var res = await _http.DeleteAsync(url);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> PostAsync(string url, object body)
    {
        var res = await _http.PostAsJsonAsync(url, body);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync(string url, object body)
    {
        var res = await _http.PutAsJsonAsync(url, body);
        return res.IsSuccessStatusCode;
    }
}