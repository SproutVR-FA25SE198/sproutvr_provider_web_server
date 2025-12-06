using Common.Domain.Exceptions;
using MediatR;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.DeleteBasket;
public class DeleteBasketCommandHandler(IBasketRepository repo) : IRequestHandler<DeleteBasketCommand>
{
    public async Task Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
    {
        bool isDeletedSuccessful = await repo.DeleteBasketAsync(request.Id);

        if (!isDeletedSuccessful)
        {
            throw new NotFoundException(nameof(Basket), request.Id);
        }
    }
}
