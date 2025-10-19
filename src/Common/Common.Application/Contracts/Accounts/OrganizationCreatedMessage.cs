namespace Common.Application.Contracts.Accounts;
public sealed class OrganizationCreatedMessage
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

