namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class OrderActivationKeyPayloadDto
{
    public string OrderId { get; set; }
    public Guid OrganizationId { get; set; }
    public List<MapPayloadDto> Maps { get; set; }
}
