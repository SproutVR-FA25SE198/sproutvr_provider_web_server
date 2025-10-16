using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
public class CreateOrganizationRequestCommandHandler(IUnitOfWork unitOfWork, IOrganizationRepository organizationRepository) : IRequestHandler<CreateOrganizationRequestCommand, OrganizationRegisterRequestDto>
{
    public async Task<OrganizationRegisterRequestDto> Handle(CreateOrganizationRequestCommand request, CancellationToken cancellationToken)
    {
        // check if org existed
        bool orgExisted = await organizationRepository.ExistsAsync(o => o.Email == request.ContactEmail || o.PhoneNumber == request.ContactPhone);
        if (orgExisted)
        {
            throw new OperationFailedException(AppCts.Errors.OrganizationRegisterRequests.Duplicated);
        }

        // check if org requested
        OrganizationRegisterRequest existedOrgRequest = await unitOfWork.Repository<OrganizationRegisterRequest>().GetEntityWithSpec(new OrganizationRequestSpecification(request.ContactEmail, request.ContactPhone));
        if (existedOrgRequest != null)
        {
            throw new OperationFailedException(AppCts.Errors.Organizations.Duplicated);
        }

        // add to db
        OrganizationRegisterRequest newOrgRequest = OrganizationRegisterRequestMappings.ToEntity(request);
        newOrgRequest.ApprovalStatus = ApprovalStatus.Unverified; // unverified email and phone

        unitOfWork.Repository<OrganizationRegisterRequest>().Add(newOrgRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // call notification service to send email-verification email - rabbitmq
        return OrganizationRegisterRequestMappings.ToDto(newOrgRequest);
    }
}
