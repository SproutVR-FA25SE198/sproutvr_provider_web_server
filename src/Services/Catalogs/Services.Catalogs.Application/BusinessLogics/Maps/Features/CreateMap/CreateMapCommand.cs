using MediatR;
using Microsoft.AspNetCore.Http;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;
#pragma warning disable CA1054 
public sealed record CreateMapCommand() : IRequest<CreateMapResponseDto>
#pragma warning restore CA1054 
{
    public IFormFile MapDataFile { get; set; }
    public string SubjectId { get; set; }
    public string Price { get; set; }
}
