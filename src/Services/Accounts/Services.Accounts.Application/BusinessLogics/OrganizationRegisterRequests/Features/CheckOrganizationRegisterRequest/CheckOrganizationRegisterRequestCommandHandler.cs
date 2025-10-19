using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Accounts;
using Common.Domain.Exceptions;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Accounts.Application.Helpers;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;
public class CheckOrganizationRegisterRequestCommandHandler(
    IUnitOfWork unitOfWork, 
    UserManager<ApplicationUser> userManager,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CheckOrganizationRegisterRequestCommand, bool>
{
    public async Task<bool> Handle(CheckOrganizationRegisterRequestCommand request, CancellationToken cancellationToken)
    {
        ApprovalStatus status = Enum.Parse<ApprovalStatus>(request.ApprovalStatus);
        
        // check if request existed
        OrganizationRegisterRequest orgRequest = await unitOfWork.Repository<OrganizationRegisterRequest>().GetByIdAsync(request.OrganizationRegisterRequestId) ?? throw new NotFoundException(AppCts.Errors.OrganizationRegisterRequests.NotFound);

        // if approve
        if (status == ApprovalStatus.Approved)
        {
            var organization = new Organization() 
            {   Name = orgRequest.OrganizationName, 
                Email = orgRequest.ContactEmail,
                PhoneNumber = orgRequest.ContactPhone,
                Address = orgRequest.Address,
                UserName = OrganizationAccountHelper.GenerateUserName(orgRequest.ContactEmail)
            };

            // add new org with initial default password
            string generatedPassword = OrganizationAccountHelper.GeneratePassword(orgRequest.OrganizationName);
            IdentityResult result = await userManager.CreateAsync(organization, generatedPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(organization, AppCts.Roles.Organization);
            }
            else
            {
                throw new OperationFailedException("Failed to add new organization!");
            }

            // send email with org info, including default password, via notification service by using rabbit mq
            var approvedMessage = new OrganizationRegisterRequestApprovedMessage() 
            { 
                OrganizationId = organization.Id.ToString(),
                Email = orgRequest.ContactEmail, 
                Name = orgRequest.OrganizationName, 
                UserName = organization.UserName, 
                Password = generatedPassword 
            };

            await publishEndpoint.Publish(approvedMessage, cancellationToken);


        }
        // if reject
        else if (status == ApprovalStatus.Rejected)
        {
            // send email with reject reason via notification service by using rabbit mq
            var approvedMessage = new OrganizationRegisterRequestRejectedMessage()
            {
                Email = orgRequest.ContactEmail,
                Name = orgRequest.OrganizationName,
                Reason = request.RejectReason
            };

            await publishEndpoint.Publish(approvedMessage, cancellationToken);
        }

        // update request status
        orgRequest.ApprovalStatus = status;
        unitOfWork.Repository<OrganizationRegisterRequest>().Update(orgRequest);
        
        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
