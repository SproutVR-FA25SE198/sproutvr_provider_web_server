namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ActivationRequestDto
{
    public Guid OrganizationId { get; set; }
    public string ActivationKey { get; set; }
}
