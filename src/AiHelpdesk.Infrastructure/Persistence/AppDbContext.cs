using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace AiHelpdesk.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantProvider currentTenantProvider)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        var tenantId = currentTenantProvider.TenantId;

        // Isolamento multi-tenant: senza contesto tenant (es. admin piattaforma, migrazioni)
        // il filtro non esclude nulla; con contesto, limita ai soli dati del tenant corrente.
        modelBuilder.Entity<Ticket>().HasQueryFilter(t => !tenantId.HasValue || t.TenantId == tenantId);
        modelBuilder.Entity<User>().HasQueryFilter(u => !tenantId.HasValue || u.TenantId == tenantId || u.TenantId == null);
        modelBuilder.Entity<TicketMessage>().HasQueryFilter(m => !tenantId.HasValue || m.Ticket!.TenantId == tenantId);
        modelBuilder.Entity<Attachment>().HasQueryFilter(a => !tenantId.HasValue || a.TicketMessage!.Ticket!.TenantId == tenantId);
    }
}
