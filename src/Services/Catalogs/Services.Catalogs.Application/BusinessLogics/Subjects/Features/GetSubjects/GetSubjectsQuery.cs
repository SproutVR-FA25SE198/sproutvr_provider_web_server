using MediatR;
using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Subjects.Features.GetSubjects;
public class GetSubjectsQuery : IRequest<IList<SubjectDto>>
{

}
