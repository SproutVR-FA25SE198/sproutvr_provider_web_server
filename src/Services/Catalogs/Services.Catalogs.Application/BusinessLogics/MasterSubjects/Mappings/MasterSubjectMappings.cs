using Services.Catalogs.Application.BusinessLogics.MasterSubjects.DTOs;
using Services.Catalogs.Domain.Entities.MasterSubjects;

namespace Services.Catalogs.Application.BusinessLogics.MasterSubjects.Mappings;

public static class MasterSubjectMappings
{
    public static MasterSubjectDto ToDto(this MasterSubject masterSubject)
    {
        return new MasterSubjectDto
        {
            Id = masterSubject.Id,
            Name = masterSubject.Name,
            Description = masterSubject.Description,
            ImageUrl = masterSubject.ImageUrl,
            Status = masterSubject.Status.ToString()
        };
    }

    public static MasterSubject ToEntity(this MasterSubjectDto dto)
    {
        return new MasterSubject
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Status = Enum.Parse<MasterSubjectStatus>(dto.Status)
        };
    }
}
