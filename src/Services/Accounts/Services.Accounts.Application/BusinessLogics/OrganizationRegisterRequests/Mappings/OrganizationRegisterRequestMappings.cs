using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
public static class OrganizationRegisterRequestMappings
{
    public static OrganizationRegisterRequest ToEntity(this CreateOrganizationRequestCommand command)
    {
        var entity = new OrganizationRegisterRequest()
        {
            OrganizationName = command.OrganizationName,
            ContactEmail = command.ContactEmail,
            ContactPhone = command.ContactPhone,
            Address = command.Address,
        };
        return entity;
    }
    public static OrganizationRegisterRequestResponseDto ToDto(this OrganizationRegisterRequest request)
    {
        var dto = new OrganizationRegisterRequestResponseDto()
        {
            Id = request.Id.ToString(),
            OrganizationName = request.OrganizationName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            ApprovalStatus = request.ApprovalStatus.ToString(),
        };
        return dto;
    } 
}
