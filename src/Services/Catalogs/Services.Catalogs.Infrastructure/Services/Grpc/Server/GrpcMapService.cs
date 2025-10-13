using CatalogsService;
using Grpc.Core;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapsByIds;

namespace Services.Catalogs.Infrastructure.Services.Grpc.Server;

public class GrpcMapService : GrpcMap.GrpcMapBase
{
    private readonly IMediator _mediator;
    public GrpcMapService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<GetMapsByIdResponse> GetMapsByIds(GetMapsByIdRequest request, ServerCallContext context)
    {
        var query = new GetMapsByIdsQuery(request.Ids.ToList());
        List<MapDto> maps = await _mediator.Send(query, context.CancellationToken);
        var response = new GetMapsByIdResponse();
        response.Maps.AddRange(maps.Select(map => new GrpcMapModel
        {
            Id = map.Id.ToString(),
            MapCode = map.MapCode,
            Name = map.Name,
            ImageUrl = map.ImageUrl,
            Price = (double)map.Price,
            SubjectName = map.Subject.Name
        }));
        return response;
    }
}
