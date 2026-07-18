using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Interfaces.Services;
using AiHelpdesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AiHelpdesk.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantProvider currentTenantProvider)
    : IdentityUserContext<ApplicationUser, Guid>(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    /// <summary>Utenti di dominio (Core.User). Nasconde deliberatamente la DbSet&lt;ApplicationUser&gt; di IdentityUserContext.</summary>
    public new DbSet<User> Users => Set<User>();
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
