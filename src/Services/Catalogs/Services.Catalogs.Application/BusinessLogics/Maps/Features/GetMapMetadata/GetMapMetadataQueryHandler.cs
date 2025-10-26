using MediatR;
using Services.Catalogs.Application.Abstractions.Services;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapMetadata;
public class GetMapMetadataQueryHandler(IMapMetadataGeneratorService mapMetadataGeneratorService) : IRequestHandler<GetMapMetadataQuery, string>
{
    public Task<string> Handle(GetMapMetadataQuery request, CancellationToken cancellationToken)
    {
        return mapMetadataGeneratorService.GenerateMapMetadataAsync(request.MapId);
    }
}
