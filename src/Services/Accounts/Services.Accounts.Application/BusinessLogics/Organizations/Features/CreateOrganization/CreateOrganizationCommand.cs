using System.ComponentModel.DataAnnotations;
using MediatR;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
public class CreateOrganizationCommand : IRequest<OrganizationDto>
{
    [Required]
    public string Name { get; set; } 
    [Required]
    public string Address { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; } 
}
