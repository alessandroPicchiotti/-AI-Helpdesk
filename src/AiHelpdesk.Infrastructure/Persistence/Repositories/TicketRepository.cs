using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Enums;
using AiHelpdesk.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiHelpdesk.Infrastructure.Persistence.Repositories;

public class TicketRepository(AppDbContext context) : ITicketRepository
{
    public Task<Ticket?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default) =>
        context.Tickets
            .Include(t => t.Messages).ThenInclude(m => m.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId, ct);

    public async Task<IReadOnlyList<Ticket>> SearchAsync(
        Guid tenantId,
        TicketAssignmentStatus? assignmentStatus = null,
        TicketWorkStatus? workStatus = null,
        TicketPriority? priority = null,
        Guid? assignedToUserId = null,
        CancellationToken ct = default)
    {
        var query = context.Tickets.Where(t => t.TenantId == tenantId);

        if (assignmentStatus.HasValue)
            query = query.Where(t => t.AssignmentStatus == assignmentStatus);
        if (workStatus.HasValue)
            query = query.Where(t => t.WorkStatus == workStatus);
        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority);
        if (assignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == assignedToUserId);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
    {
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken ct = default)
    {
        context.Tickets.Update(ticket);
        await context.SaveChangesAsync(ct);
    }

    public async Task AddMessageAsync(TicketMessage message, CancellationToken ct = default)
    {
        context.TicketMessages.Add(message);
        await context.SaveChangesAsync(ct);
    }
}
