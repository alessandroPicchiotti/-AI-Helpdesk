using AiHelpdesk.Core.Enums;

namespace AiHelpdesk.Core.Entities;

public class User
{
    public Guid Id { get; set; }

    /// <summary>Null solo per l'admin di piattaforma, che non appartiene a un singolo tenant.</summary>
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public UserRole Role { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
