using AiHelpdesk.Core.Entities;
using AiHelpdesk.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiHelpdesk.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, Guid? tenantId, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId, ct);

    public async Task<IReadOnlyList<User>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default) =>
        await context.Users.Where(u => u.TenantId == tenantId).ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }
}
