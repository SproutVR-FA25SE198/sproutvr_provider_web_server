namespace Services.Catalogs.Application.BusinessLogics.MapObjects.DTOs;

public class MapObjectDto
{
    public Guid Id { get; set; }
    public Guid MapId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string ObjectCode { get; set; }
}
