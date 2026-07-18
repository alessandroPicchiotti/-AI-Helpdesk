namespace AiHelpdesk.Core.Interfaces.Services;

public record TicketClassificationResult(string ProductCategory, double Confidence);

public record TicketSuggestedReply(string Text, double Confidence);

public interface IAiClassificationService
{
    Task<TicketClassificationResult> ClassifyAsync(string subject, string description, CancellationToken ct = default);

    Task<string> SummarizeAsync(IReadOnlyList<string> messageBodies, CancellationToken ct = default);

    Task<IReadOnlyList<TicketSuggestedReply>> SuggestRepliesAsync(
        string subject,
        IReadOnlyList<string> messageBodies,
        IReadOnlyList<string> knowledgeBaseSnippets,
        CancellationToken ct = default);
}
