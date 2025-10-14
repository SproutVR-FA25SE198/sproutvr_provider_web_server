using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
public class CreateOrganizationCommand : IRequest
{
    [Required]
    public string Name { get; set; } 
    [Required]
    public string Address { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; } 
    [Required]
    public string RepresentativeName { get; set; }
}
