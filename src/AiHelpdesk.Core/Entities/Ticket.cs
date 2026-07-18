using AiHelpdesk.Core.Enums;

namespace AiHelpdesk.Core.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public required string Subject { get; set; }
    public required string Description { get; set; }
    public TicketPriority Priority { get; set; }

    public TicketAssignmentStatus AssignmentStatus { get; set; } = TicketAssignmentStatus.NonAssegnato;
    public TicketWorkStatus WorkStatus { get; set; } = TicketWorkStatus.InLavorazione;

    public Guid CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    /// <summary>Tipologia di prodotto identificata dalla classificazione AI.</summary>
    public string? ProductCategory { get; set; }
    public string? AiSummary { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    public ICollection<TicketMessage> Messages { get; set; } = [];
}
