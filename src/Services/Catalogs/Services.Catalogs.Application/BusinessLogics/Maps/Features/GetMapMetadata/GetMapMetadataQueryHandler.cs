using MediatR;
using Services.Catalogs.Application.Abstractions.Services;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapMetadata;
public class GetMapMetadataQueryHandler(IMapMetadataService mapMetadataService) : IRequestHandler<GetMapMetadataQuery, string>
{
    public Task<string> Handle(GetMapMetadataQuery request, CancellationToken cancellationToken)
    {
        return mapMetadataService.GenerateMapMetadataAsync(request.MapId);
    }
}
