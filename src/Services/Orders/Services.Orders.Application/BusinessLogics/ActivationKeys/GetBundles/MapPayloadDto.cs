namespace Services.Orders.Application.BusinessLogics.ActivationKeys.GetBundles;
public class MapPayloadDto
{
    public Guid OrderItemId { get; set; }
    public string MapCode { get; set; }
    public string MapName { get; set; }
    public string ImageUrl { get; set; }
    public string DownloadUrl { get; set; }
}
