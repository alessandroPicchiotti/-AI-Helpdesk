using AiHelpdesk.Core.Entities;

namespace AiHelpdesk.Core.Interfaces.Services;

public interface INotificationService
{
    /// <summary>Notifica admin tenant e ruolo ManageHelpdesk quando arriva un nuovo ticket non assegnato.</summary>
    Task NotifyNewUnassignedTicketAsync(Ticket ticket, CancellationToken ct = default);

    Task NotifyTicketStatusChangedAsync(Ticket ticket, CancellationToken ct = default);

    Task NotifyNewMessageAsync(Ticket ticket, TicketMessage message, CancellationToken ct = default);
}
