namespace Frontend.Domain.Contracts;
public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
}