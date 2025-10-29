namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class OrderActivationKeyPayloadDto
{
    public string OrderId { get; set; }
    public Guid OrganizationId { get; set; }

#pragma warning disable S1135
    // TODO: ADD DOWNLOAD URLS IN THE PAYLOAD
}
