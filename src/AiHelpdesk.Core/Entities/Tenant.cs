namespace AiHelpdesk.Core.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Ticket> Tickets { get; set; } = [];
}
