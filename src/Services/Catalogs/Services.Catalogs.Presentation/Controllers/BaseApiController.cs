using Common.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Services.Catalogs.Presentation.Controllers;

[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
#pragma warning disable CA1515 // Consider making public types internal
public abstract class BaseApiController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    internal ActionResult PaginationOkResult<T>(IReadOnlyList<T> items, int count, int pageIndex, int pageSize)
    {
        var pagination = new PaginatedResult<T>(pageIndex, pageSize, count, items);
        return Ok(pagination);
    }
}
