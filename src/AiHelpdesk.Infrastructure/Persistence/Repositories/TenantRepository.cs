using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiHelpdesk.Infrastructure.Persistence.Repositories;

public class TenantRepository(AppDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default) =>
        await context.Tenants.ToListAsync(ct);

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
    {
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        context.Tenants.Update(tenant);
        await context.SaveChangesAsync(ct);
    }
}
