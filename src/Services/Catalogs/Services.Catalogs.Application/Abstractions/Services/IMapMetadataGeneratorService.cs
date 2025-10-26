
namespace Services.Catalogs.Application.Abstractions.Services;

public interface IMapMetadataGeneratorService
{
    Task<string> GenerateMapMetadataAsync(Guid mapId);
}
