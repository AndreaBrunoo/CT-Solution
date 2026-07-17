namespace Frontend.Infrastructure.Api;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url);
    Task<T?> PostAsync<T>(string url, object body);
    Task<T?> PutAsync<T>(string url, object body);
    Task<bool> PostAsync(string url, object body);
    Task<bool> PutAsync(string url, object body);
    Task<bool> DeleteAsync(string url);
}