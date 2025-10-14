using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
public class CreateOrganizationRequestCommand : IRequest<OrganizationRegisterRequestResponseDto>
{
    [Required]
    public string OrganizationName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string ContactPhone { get; set; }
    [Required]
    public string ContactEmail { get; set; }
    [Required]
    public string RepresentativeName { get; set; }
}
