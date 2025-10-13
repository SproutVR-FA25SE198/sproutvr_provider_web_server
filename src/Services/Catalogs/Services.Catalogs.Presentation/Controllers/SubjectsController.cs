using Common.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.Subjects.Features.GetSubjects;

namespace Services.Catalogs.Presentation.Controllers;
[Route("api/catalogs/subjects")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class SubjectsController(IMediator mediator) : BaseApiController
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet]
    public async Task<IActionResult> GetMaps(CancellationToken cancellationToken)
    {
        var query = new GetSubjectsQuery();
        IList<SubjectDto> result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
