using CatalogsService;
using Microsoft.Extensions.Logging;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

namespace Services.Orders.Infrastructure.Services.Grpc.Client;
public class GrpcMapClient : IGrpcMapClient
{
    private readonly ILogger<GrpcMapClient> _logger;
    private readonly GrpcMap.GrpcMapClient _client;

    public GrpcMapClient(ILogger<GrpcMapClient> logger,
        GrpcMap.GrpcMapClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<IReadOnlyList<MapDto>> GetMapsByIdsAsync(List<string> ids)
    {
        _logger.LogInformation("Calling GRPC Service to get maps list");

        // Correctly assign the ids to the request's Ids property
        var request = new GetMapsByIdRequest();
        request.Ids.AddRange(ids);

        try
        {
            // Make a request to Grpc Server
            GetMapsByIdResponse response = await _client.GetMapsByIdsAsync(request);

            // Map the grpc maps response to the list of maps
            var maps = response.Maps.Select(map => new MapDto
            {
                MapId = Guid.Parse(map.Id),
                MapName = map.Name,
                MapCode = map.MapCode,
                Price = (decimal)map.Price,
                ImageUrl = map.ImageUrl
            }).ToList();
            return maps;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not call Grpc Server");
            return [];
        }
    }
}

