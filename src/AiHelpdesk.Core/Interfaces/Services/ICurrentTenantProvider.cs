namespace AiHelpdesk.Core.Interfaces.Services;

/// <summary>
/// Espone il tenant della richiesta corrente (risolto dai claim JWT), usato dal DbContext
/// per applicare l'isolamento multi-tenant. Null per contesti senza tenant (es. admin di piattaforma).
/// </summary>
public interface ICurrentTenantProvider
{
    Guid? TenantId { get; }
}
