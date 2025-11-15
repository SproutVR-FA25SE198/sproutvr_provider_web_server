namespace Common.Application.Contracts.Accounts;

public sealed class OrganizationRequestCreatedMessage
{
    public string OrganizationName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string VerificationToken { get; set; } = string.Empty;
    public Guid OrganizationRegisterRequestId { get; set; }
}

