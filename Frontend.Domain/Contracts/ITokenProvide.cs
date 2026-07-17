namespace Frontend.Domain.Contracts;
public interface ITokenProvider
{
    Task<string?> GetTokenAsync();
}