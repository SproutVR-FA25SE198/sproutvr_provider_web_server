using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.VerifyEmail;

public class VerifyEmailCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<VerifyEmailCommand, bool>
{
    public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        // Get the organization register request
        OrganizationRegisterRequest? orgRequest = await unitOfWork
            .Repository<OrganizationRegisterRequest>()
            .GetByIdAsync(request.OrganizationRegisterRequestId);

        if (orgRequest == null)
        {
            throw new NotFoundException(AppCts.Errors.OrganizationRegisterRequests.NotFound);
        }

        // Check if already verified
        if (orgRequest.IsEmailVerified)
        {
            throw new OperationFailedException("Email đã được xác nhận trước đó.");
        }

        // Check if token matches
        if (orgRequest.EmailVerificationToken != request.Token)
        {
            throw new OperationFailedException("Mã xác nhận không hợp lệ.");
        }

        // Check if token has expired
        if (orgRequest.EmailVerificationTokenExpiry == null || 
            orgRequest.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            throw new OperationFailedException("Mã xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại email xác nhận.");
        }

        // Mark email as verified
        orgRequest.IsEmailVerified = true;
        orgRequest.EmailVerificationToken = null; // Clear token after successful verification
        orgRequest.EmailVerificationTokenExpiry = null;
        
        // Update status to Pending (waiting for admin approval)
        orgRequest.ApprovalStatus = ApprovalStatus.Approval_Pending;

        unitOfWork.Repository<OrganizationRegisterRequest>().Update(orgRequest);
        
        bool saved = await unitOfWork.SaveChangesAsync(cancellationToken);

        if (!saved)
        {
            throw new OperationFailedException("Không thể xác nhận email. Vui lòng thử lại.");
        }

        return true;
    }
}

