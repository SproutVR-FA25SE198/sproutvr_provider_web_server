namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class MapPayloadDto
{
    public Guid MapId { get; set; }
    public string MapCode { get; set; }
    public string MapName { get; set; }
    public string ImageUrl { get; set; }
    public string DownloadUrl { get; set; }
}
