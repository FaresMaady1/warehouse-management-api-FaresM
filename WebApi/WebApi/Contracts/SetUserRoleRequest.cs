namespace WebApi.Contracts;

public class SetUserRoleRequest
{
    public string Uid { get; set; } = default!;
    public string Role { get; set; } = default!; 
}