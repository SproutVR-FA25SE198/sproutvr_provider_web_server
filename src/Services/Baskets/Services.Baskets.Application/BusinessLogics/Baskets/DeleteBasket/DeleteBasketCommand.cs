using MediatR;

namespace Services.Baskets.Application.BusinessLogics.Baskets.DeleteBasket;
public class DeleteBasketCommand : IRequest
{
    public string Id { get; }

    public DeleteBasketCommand(string id)
    {
        Id = id;
    }
}
