using AiHelpdesk.Core.Entities;

namespace AiHelpdesk.Core.Interfaces.Services;

/// <summary>
/// Emette il JWT di accesso per un utente autenticato, con i claim custom TenantId e Ruolo
/// usati per l'isolamento multi-tenant e l'autorizzazione basata sui ruoli.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
