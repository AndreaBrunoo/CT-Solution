using Microsoft.AspNetCore.Components;
using Frontend.Application.Services;
using Frontend.Domain.Contracts;

namespace Frontend.Presentation.Helpers;

/// <summary>
/// Guard centralizzato per il redirect a / quando l'utente non è loggato
/// oppure non ha i ruoli richiesti.
/// </summary>
public static class AuthGuard
{
    /// <summary>Restituisce true se l'utente è loggato, altrimenti redirige a /.</summary>
    public static async Task<bool> EnsureAuthenticatedAsync(
        CurrentUserService current, NavigationManager nav, ITokenProvider? tokens = null)
    {
        if (current.IsAuthenticated) return true;

        // Provo a idratare (es. F5 dopo login): se c'è un token valido, idratiamo
        var hydrated = await current.TryHydrateAsync();
        if (hydrated) return true;

        // Niente token o non valido → login
        nav.NavigateTo("/", forceLoad: false);
        return false;
    }

    /// <summary>True se l'utente ha ALMENO uno dei ruoli indicati. Altrimenti redirige a /Dashboard.</summary>
    public static async Task<bool> EnsureRoleAsync(
        CurrentUserService current, NavigationManager nav, params string[] anyRole)
    {
        if (!await EnsureAuthenticatedAsync(current, nav)) return false;

        if (current.IsInAnyRole(anyRole)) return true;

        nav.NavigateTo("/Dashboard", forceLoad: false);
        return false;
    }
}
