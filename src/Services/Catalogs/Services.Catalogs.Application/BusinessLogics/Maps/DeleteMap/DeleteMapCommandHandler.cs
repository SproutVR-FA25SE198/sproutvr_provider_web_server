using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Catalogs.Domain.Entities.Maps;

namespace Services.Catalogs.Application.BusinessLogics.Maps.DeleteMap;

public sealed class DeleteMapCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteMapCommand>
{
    public async Task Handle(DeleteMapCommand request, CancellationToken cancellationToken)
    {
        Map map = await unitOfWork.Repository<Map>().GetByIdAsync(request.Id);
        
        if (map == null)
        {
            throw new NotFoundException("Map not found!");
        }

        unitOfWork.Repository<Map>().Delete(map);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
