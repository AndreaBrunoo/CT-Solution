using Frontend.Domain.Contracts;
using Frontend.Domain.Dtos.User;

namespace Frontend.Application.Services;

/// <summary>
/// Mantiene in cache l'utente corrente della sessione WebAssembly.
/// Viene popolato al login (vedi Login.razor) e svuotato al logout.
/// Espone helper per i controlli di autorizzazione usati dalle pagine.
/// </summary>
public class CurrentUserService
{
    private readonly IUserService _userService;
    private readonly ITokenProvider _tokenProvider;

    private CurrentUserDto? _current;
    public CurrentUserDto? Current => _current;
    public bool IsAuthenticated => _current is not null;

    public event Action? Changed;

    public CurrentUserService(IUserService userService, ITokenProvider tokenProvider)
    {
        _userService = userService;
        _tokenProvider = tokenProvider;
    }

    /// <summary>
    /// Tenta di idratare lo stato leggendo il token da localStorage.
    /// Chiamato all'avvio dell'app: ritorna false se non c'è token.
    /// </summary>
    public async Task<bool> TryHydrateAsync()
    {
        var token = await _tokenProvider.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            _current = await _userService.GetCurrentUserAsync();
            if (_current is not null) Changed?.Invoke();
            return _current is not null;
        }
        catch
        {
            // Token scaduto o non valido: pulisco per evitare loop
            await _tokenProvider.RemoveTokenAsync();
            _current = null;
            return false;
        }
    }

    public async Task SetAsync(CurrentUserDto user)
    {
        _current = user;
        Changed?.Invoke();
        await Task.CompletedTask;
    }

    public async Task ClearAsync()
    {
        _current = null;
        await _tokenProvider.RemoveTokenAsync();
        Changed?.Invoke();
    }

    public bool IsInRole(string role)
        => _current?.Roles is { Count: > 0 } && _current.Roles.Contains(role);

    /// <summary>
    /// True se l'utente ha ALMENO uno dei ruoli indicati.
    /// </summary>
    public bool IsInAnyRole(params string[] roles)
        => _current?.Roles is { Count: > 0 } rolesList
           && rolesList.Any(r => roles.Contains(r));
}
