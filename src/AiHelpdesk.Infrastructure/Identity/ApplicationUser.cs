using Microsoft.AspNetCore.Identity;

namespace AiHelpdesk.Infrastructure.Identity;

/// <summary>
/// Utente ASP.NET Identity: gestisce solo le credenziali (password hash, security stamp, lockout).
/// I dati di dominio (TenantId, Ruolo, DisplayName) restano su <see cref="AiHelpdesk.Core.Entities.User"/>,
/// che condivide lo stesso Id ma non ha una relazione EF diretta con questa entità.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>;
