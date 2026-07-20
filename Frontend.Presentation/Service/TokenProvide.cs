using Microsoft.JSInterop;
using Frontend.Domain.Contracts;

namespace Frontend.Presentation.Service;
public class TokenProvider : ITokenProvider
{
    private readonly IJSRuntime _js;

    public TokenProvider(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>("localStorageInterop.get", "authToken");
    }

    public async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorageInterop.set", "authToken", token);
    }

    public async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorageInterop.remove", "authToken");
    }
}
