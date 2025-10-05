using Common.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderById;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;

namespace Services.Orders.Presentation.Controllers;
[ApiController]
[Route("api/orders")]
#pragma warning disable CA1515 // Consider making public types internal
public class OrdersController(IMediator mediator) : BaseApiController
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderParams orderParams, CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery(orderParams);
        PaginatedResult<OrderDto> result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        OrderDetailsDto result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto order)
    {
        OrderResponseDto response = await mediator.Send(new CreateOrderCommand(order));
        return Ok(response);
    }
    
}
