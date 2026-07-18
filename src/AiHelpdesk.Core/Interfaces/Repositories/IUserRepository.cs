using AiHelpdesk.Core.Entities;

namespace AiHelpdesk.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, Guid? tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}
