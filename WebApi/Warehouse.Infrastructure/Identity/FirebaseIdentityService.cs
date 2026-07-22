namespace Warehouse.Infrastructure.Identity;

using FirebaseAdmin.Auth;
using Warehouse.Domain.Identity;

public class FirebaseIdentityService : IIdentityService
{
    public Task SetUserRoleAsync(string uid, string role, CancellationToken cancellationToken = default) =>
        FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, new Dictionary<string, object> { ["role"] = role });
}