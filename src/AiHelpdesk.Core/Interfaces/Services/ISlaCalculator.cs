using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Enums;

namespace AiHelpdesk.Core.Interfaces.Services;

public interface ISlaCalculator
{
    /// <summary>Tempo di prima risposta atteso per la priorità indicata.</summary>
    TimeSpan GetTargetFirstResponseTime(TicketPriority priority);

    /// <summary>
    /// Tempo trascorso valido ai fini SLA, sospeso durante gli intervalli in cui il ticket
    /// è rimasto in stato "InAttesaCliente".
    /// </summary>
    TimeSpan GetElapsedSlaTime(Ticket ticket);

    bool IsSlaBreached(Ticket ticket);
}
