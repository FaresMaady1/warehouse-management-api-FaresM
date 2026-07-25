namespace Warehouse.Application.Commands.SetUserRole;

using MediatR;
using Warehouse.Domain.Identity;

public class SetUserRoleHandler : IRequestHandler<SetUserRoleCommand>
{
    private readonly IIdentityService _identityService;
    public SetUserRoleHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task Handle(SetUserRoleCommand request, CancellationToken cancellationToken) =>
        await _identityService.SetUserRoleAsync(request.Uid, request.Role, cancellationToken);
}