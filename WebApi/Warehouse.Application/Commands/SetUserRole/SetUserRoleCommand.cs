namespace Warehouse.Application.Commands.SetUserRole;

using MediatR;

public record SetUserRoleCommand(string Uid, string Role) : IRequest;