namespace AiHelpdesk.Core.Entities;

public class Attachment
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    public Guid Id { get; set; }
    public Guid TicketMessageId { get; set; }
    public TicketMessage? TicketMessage { get; set; }

    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeBytes { get; set; }

    /// <summary>Percorso relativo sul filesystem del server dove è salvato il file.</summary>
    public required string StoragePath { get; set; }

    public DateTimeOffset UploadedAt { get; set; }
}
