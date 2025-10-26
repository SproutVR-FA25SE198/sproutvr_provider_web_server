using MediatR;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapMetadata;
public class GetMapMetadataQuery : IRequest<string>
{
    public Guid MapId { get; }
    public GetMapMetadataQuery(Guid mapId)
    {
        MapId = mapId;
    }
}
