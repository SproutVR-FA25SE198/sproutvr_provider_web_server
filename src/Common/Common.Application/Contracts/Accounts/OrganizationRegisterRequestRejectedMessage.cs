namespace Common.Application.Contracts.Accounts;
public class OrganizationRegisterRequestRejectedMessage
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Reason { get; set; }
}
