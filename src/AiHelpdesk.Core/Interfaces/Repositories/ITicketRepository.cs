using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Enums;

namespace AiHelpdesk.Core.Interfaces.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);

    Task<IReadOnlyList<Ticket>> SearchAsync(
        Guid tenantId,
        TicketAssignmentStatus? assignmentStatus = null,
        TicketWorkStatus? workStatus = null,
        TicketPriority? priority = null,
        Guid? assignedToUserId = null,
        CancellationToken ct = default);

    Task AddAsync(Ticket ticket, CancellationToken ct = default);
    Task UpdateAsync(Ticket ticket, CancellationToken ct = default);
    Task AddMessageAsync(TicketMessage message, CancellationToken ct = default);
}
