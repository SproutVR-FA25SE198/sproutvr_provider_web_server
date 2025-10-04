using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Baskets.Application.BusinessLogics.Baskets.DeleteBasket;
using Services.Baskets.Application.BusinessLogics.Baskets.GetBasketById;
using Services.Baskets.Application.BusinessLogics.Baskets.UpdateBasket;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
#pragma warning disable CA1515 // Consider making public types internal
public class BasketsController(IMediator mediator) : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet("{basketId}")]
    public async Task<ActionResult<Basket>> GetBasketById([FromRoute] string basketId)
    {
        Basket basket = await mediator.Send(new GetBasketByIdQuery(basketId));
        return Ok(basket);
    }

    [HttpPost]
    public async Task<ActionResult<Basket>> UpdateBasket([FromBody] Basket basket)
    {
        Basket updatedBasket = await mediator.Send(new UpdateBasketCommand(basket));
        return Ok(updatedBasket);
    }

    [HttpDelete("{basketId}")]
    public async Task<ActionResult> DeleteBasket([FromRoute] string basketId)
    {
        await mediator.Send(new DeleteBasketCommand(basketId));
        return NoContent();
    }
}
