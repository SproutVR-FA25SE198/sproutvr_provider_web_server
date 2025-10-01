namespace Services.Catalogs.Application.BusinessLogics.MasterSubjects.DTOs;

public class MasterSubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
