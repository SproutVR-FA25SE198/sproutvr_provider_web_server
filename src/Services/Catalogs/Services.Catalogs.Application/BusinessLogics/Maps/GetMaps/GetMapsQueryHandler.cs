using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;
using Services.Catalogs.Application.BusinessLogics.Maps.Mappings;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.GetMaps;

public sealed class GetMapsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMapsQuery, PaginatedResult<MapDto>>
{
    public async Task<PaginatedResult<MapDto>> Handle(GetMapsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Map> mapList = await unitOfWork.Repository<Map>().ListAsync(new MapSpecification(request.SpecParams));
        int totalCount = await unitOfWork.Repository<Map>().CountAsync(new MapSpecification(request.SpecParams));
        List<MapDto> result = mapList.Any() ? mapList.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<MapDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
