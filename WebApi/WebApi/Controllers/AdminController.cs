namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using Warehouse.Application.Commands.SetUserRole;

[ApiController]
[Route("api/admin")]
[AllowAnonymous] 
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AdminController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("users/role")]
    public async Task<IActionResult> SetRole(
        [FromBody] SetUserRoleRequest request,
        [FromHeader(Name = "X-Bootstrap-Key")] string? bootstrapKey,
        CancellationToken cancellationToken)
    {
        var expectedKey = _configuration["Firebase:BootstrapAdminKey"];
        if (string.IsNullOrEmpty(expectedKey) || bootstrapKey != expectedKey)
            return Unauthorized("Invalid or missing bootstrap key.");

        await _mediator.Send(new SetUserRoleCommand(request.Uid, request.Role), cancellationToken);
        return NoContent();
    }
}