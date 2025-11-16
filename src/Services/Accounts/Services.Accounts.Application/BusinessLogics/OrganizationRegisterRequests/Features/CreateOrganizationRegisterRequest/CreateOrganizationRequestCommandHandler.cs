using System.Security.Cryptography;
using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Accounts;
using Common.Domain.Exceptions;
using MassTransit;
using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
public class CreateOrganizationRequestCommandHandler(
    IUnitOfWork unitOfWork, 
    IOrganizationRepository organizationRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<CreateOrganizationRequestCommand, OrganizationRegisterRequestDto>
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
        newOrgRequest.ApprovalStatus = ApprovalStatus.Unverified; // unverified email
        
        // Generate email verification token (secure random string)
        newOrgRequest.EmailVerificationToken = GenerateSecureToken();
        newOrgRequest.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24); // Token valid for 24 hours
        newOrgRequest.IsEmailVerified = false;

        unitOfWork.Repository<OrganizationRegisterRequest>().Add(newOrgRequest);


        // Publish message to RabbitMQ for sending verification email via Notification service
        var verificationMessage = new OrganizationRequestCreatedMessage
        {
            OrganizationName = newOrgRequest.OrganizationName,
            Email = newOrgRequest.ContactEmail,
            VerificationToken = newOrgRequest.EmailVerificationToken,
            OrganizationRegisterRequestId = newOrgRequest.Id
        };

        await publishEndpoint.Publish(verificationMessage, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OrganizationRegisterRequestMappings.ToDto(newOrgRequest);
    }

    private static string GenerateSecureToken()
    {
        // Generate a cryptographically secure random token
        byte[] randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
