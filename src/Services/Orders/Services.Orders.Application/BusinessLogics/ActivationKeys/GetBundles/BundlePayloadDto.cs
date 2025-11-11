namespace Services.Orders.Application.BusinessLogics.ActivationKeys.GetBundles;
public class BundlePayloadDto
{
    public string OrderId { get; set; }
    public Guid OrganizationId { get; set; }
    public List<MapPayloadDto> Maps { get; set; }
}
