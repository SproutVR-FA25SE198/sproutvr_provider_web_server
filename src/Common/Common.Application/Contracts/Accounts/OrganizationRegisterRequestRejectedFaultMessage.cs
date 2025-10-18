namespace Common.Application.Contracts.Accounts;
public class OrganizationRegisterRequestRejectedFaultMessage
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Reason { get; set; }
    public string ErrorMessage { get; set; }
}
