using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
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
    UserManager<ApplicationUser> userManager) : IRequestHandler<CheckOrganizationRegisterRequestCommand, bool>
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
            IdentityResult result = await userManager.CreateAsync(organization, OrganizationAccountHelper.GeneratePassword(orgRequest.OrganizationName));
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(organization, AppCts.Roles.Organization);
            }
            else
            {
                throw new OperationFailedException("Failed to add new organization!");
            }

            // send email with org info, including default password, via notification service by using rabbit mq

        }
        // if reject
        else if (status == ApprovalStatus.Rejected)
        {
            // send email via notification service by using rabbit mq
        }

        // update request status
        orgRequest.ApprovalStatus = status;
        unitOfWork.Repository<OrganizationRegisterRequest>().Update(orgRequest);
        
        return await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
