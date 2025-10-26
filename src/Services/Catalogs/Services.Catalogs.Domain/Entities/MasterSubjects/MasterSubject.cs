using System.Text.Json.Serialization;
using Common.Domain.Entities;
using Services.Catalogs.Domain.Entities.Subjects;

namespace Services.Catalogs.Domain.Entities.MasterSubjects;
public class MasterSubject : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public MasterSubjectStatus Status { get; set; }
    [JsonIgnore]
    public List<Subject> Subjects { get; set; } = [];
}
