using Common.Domain.Entities;

namespace Services.Catalogs.Application.Abstractions.Services;

public interface IMapMetadataService
{
    Task<string> GenerateMapMetadataAsync(Guid mapId);
    Task<Guid?> SeedSingleFileForMapBundleAsync<T>(string absoluteFilePath) where T : BaseEntity;

}
