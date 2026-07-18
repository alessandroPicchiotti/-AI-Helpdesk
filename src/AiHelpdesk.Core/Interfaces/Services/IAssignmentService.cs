using AiHelpdesk.Core.Entities;

namespace AiHelpdesk.Core.Interfaces.Services;

public interface IAssignmentService
{
    /// <summary>
    /// Assegna un ticket appena creato: automaticamente se il cliente ha indicato l'email
    /// di un operatore esistente nel tenant, altrimenti lo lascia "non assegnato" e notifica
    /// admin/ManageHelpdesk.
    /// </summary>
    Task AssignOnCreationAsync(Ticket ticket, string? requestedOperatorEmail, CancellationToken ct = default);

    Task AssignToOperatorAsync(Guid ticketId, Guid operatorUserId, Guid assignedByUserId, CancellationToken ct = default);

    Task ReassignAsync(Guid ticketId, Guid newOperatorUserId, Guid reassignedByUserId, CancellationToken ct = default);
}
