using System.ComponentModel.DataAnnotations;
using MediatR;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
public class CreateOrganizationRequestCommand : IRequest<OrganizationRegisterRequestDto>
{
    [Required]
    public string OrganizationName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string ContactPhone { get; set; }
    [Required]
    public string ContactEmail { get; set; }
}
