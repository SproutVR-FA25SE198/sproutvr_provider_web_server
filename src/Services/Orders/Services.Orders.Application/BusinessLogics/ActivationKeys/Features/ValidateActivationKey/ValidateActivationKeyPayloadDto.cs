namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;
public class ValidateActivationKeyPayloadDto
{
    public Guid OrderId { get; set; }
    public Guid OrganizationId { get; set; }
}
