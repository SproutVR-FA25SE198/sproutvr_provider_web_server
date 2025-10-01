using Services.Catalogs.Application.BusinessLogics.MasterSubjects.DTOs;

namespace Services.Catalogs.Application.BusinessLogics.Subjects.DTOs;

public class SubjectDto
{
    public Guid Id { get; set; }
    public MasterSubjectDto MasterSubject { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
