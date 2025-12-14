using FluentValidation;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;

public class CheckOrganizationRegisterRequestCommandValidator : AbstractValidator<CheckOrganizationRegisterRequestCommand>
{
    public CheckOrganizationRegisterRequestCommandValidator()
    {
        RuleFor(x => x.OrganizationRegisterRequestId)
            .NotEmpty().WithMessage("Organization register request ID is required");

        RuleFor(x => x.ApprovalStatus)
            .NotEmpty().WithMessage("Approval status is required")
            .Must(status => status == "Approved" || status == "Rejected")
            .WithMessage("Approval status must be either 'Approved' or 'Rejected'");

        RuleFor(x => x.RejectReason)
            .NotEmpty().WithMessage("Reject reason is required when status is Rejected")
            .When(x => x.ApprovalStatus == "Rejected");
    }
}

