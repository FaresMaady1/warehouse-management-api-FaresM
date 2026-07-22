namespace Warehouse.Domain.Identity;

public interface IIdentityService
{
    Task SetUserRoleAsync(string uid, string role, CancellationToken cancellationToken = default);
}