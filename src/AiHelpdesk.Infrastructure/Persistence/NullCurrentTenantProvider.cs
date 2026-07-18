using AiHelpdesk.Core.Interfaces.Services;

namespace AiHelpdesk.Infrastructure.Persistence;

/// <summary>
/// Provider di default privo di contesto tenant, usato per migrazioni, seed e strumenti offline.
/// Nella pipeline HTTP dell'API viene sostituito da un'implementazione basata sui claim JWT (Fase 2).
/// </summary>
public sealed class NullCurrentTenantProvider : ICurrentTenantProvider
{
    public Guid? TenantId => null;
}
