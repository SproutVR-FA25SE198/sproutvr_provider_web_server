using Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;
using Services.Catalogs.Application.BusinessLogics.MasterSubjects.Mappings;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Application.BusinessLogics.MasterSubjects.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Subjects.Mappings;

public static class SubjectMappings
{
    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto
        {
            Id = subject.Id,
            MasterSubject = subject.MasterSubject?.ToDto() ?? new MasterSubjectDto(),
            Name = subject.Name,
            Description = subject.Description,
            ImageUrl = subject.ImageUrl,
            Status = subject.Status.ToString()
        };
    }

    public static Subject ToEntity(this SubjectDto dto)
    {
        return new Subject
        {
            Id = dto.Id,
            MasterSubjectId = dto.MasterSubject.Id,
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Status = Enum.Parse<SubjectStatus>(dto.Status)
        };
    }
}
