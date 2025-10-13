using Common.Application.Helpers;
using Services.Catalogs.Domain.Entities.Subjects;

namespace Services.Catalogs.Application.BusinessLogics.Subjects.Specifications;
public class SubjectSpecification : BaseSpecification<Subject>
{
    public SubjectSpecification()
        : base()
    {
        AddInclude(s => s.MasterSubject);
    }
}
