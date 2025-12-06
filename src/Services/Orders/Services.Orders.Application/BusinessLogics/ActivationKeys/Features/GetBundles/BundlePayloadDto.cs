namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
public class BundlePayloadDto
{
    public Guid OrderId { get; set; }
    public long? OrderCode { get; set; }
    public Guid OrganizationId { get; set; }
    public int MapCount { get; set; }
    public List<MapPayloadDto> Maps { get; set; }
}
