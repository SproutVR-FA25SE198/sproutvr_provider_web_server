using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.MasterSubjects;

namespace Services.Catalogs.Domain.Entities.Subjects;
public class Subject : BaseEntity
{
    public Guid MasterSubjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public SubjectStatus Status { get; set; }

    // navigation property
    public MasterSubject MasterSubject { get; set; }
    public List<Map> Maps { get; set; } = [];
}
