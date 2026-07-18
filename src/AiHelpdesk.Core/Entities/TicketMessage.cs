using AiHelpdesk.Core.Enums;

namespace AiHelpdesk.Core.Entities;

/// <summary>Voce della cronologia unica di comunicazione di un ticket (email, risposta AI, chat).</summary>
public class TicketMessage
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    /// <summary>Null quando il messaggio è generato dall'AI.</summary>
    public Guid? AuthorUserId { get; set; }
    public User? AuthorUser { get; set; }

    public TicketMessageChannel Channel { get; set; }
    public required string Body { get; set; }
    public DateTimeOffset SentAt { get; set; }

    public ICollection<Attachment> Attachments { get; set; } = [];
}
