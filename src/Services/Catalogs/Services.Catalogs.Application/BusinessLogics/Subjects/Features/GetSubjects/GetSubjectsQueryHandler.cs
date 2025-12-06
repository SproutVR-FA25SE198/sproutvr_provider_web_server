using Common.Application.Abstractions.Data;
using MassTransit.Initializers;
using MediatR;
using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.Subjects.Mappings;
using Services.Catalogs.Application.BusinessLogics.Subjects.Specifications;
using Services.Catalogs.Domain.Entities.Subjects;

namespace Services.Catalogs.Application.BusinessLogics.Subjects.Features.GetSubjects;
public class GetSubjectsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSubjectsQuery, IList<SubjectDto>>
{
    public async Task<IList<SubjectDto>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Subject> subjectList = await unitOfWork.Repository<Subject>().ListAsync(new SubjectSpecification());      
        return subjectList.Select(s => SubjectMappings.ToDto(s)).ToList();
    }
}
