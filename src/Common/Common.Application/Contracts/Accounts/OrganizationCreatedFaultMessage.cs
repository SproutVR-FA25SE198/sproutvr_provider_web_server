namespace Common.Application.Contracts.Accounts;
public sealed class OrganizationCreatedFaultMessage
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

