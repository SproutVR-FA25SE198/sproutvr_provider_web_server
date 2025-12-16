using FluentValidation;

namespace Services.Bundles.Application.BusinessLogics.UploadBundle;

public class UploadBundleCommandValidator : AbstractValidator<UploadBundleCommand>
{
    public UploadBundleCommandValidator()
    {
        RuleFor(x => x.BundleFile)
            .NotNull().WithMessage("Bundle file is required")
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Bundle file cannot be empty")
            .Must(file => file == null || file.FileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase))
            .WithMessage("Bundle file must be a ZIP file");

        RuleFor(x => x.OrderDto)
            .NotNull().WithMessage("Order data is required")
            .SetValidator(new OrderDtoValidator());
    }
}

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.OrderCode)
            .GreaterThan(0).WithMessage("Order code must be greater than 0");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.BundleGoogleDriveId)
            .NotEmpty().WithMessage("Bundle Google Drive ID is required");

        RuleFor(x => x.AssignedSystemAdminId)
            .NotEmpty().WithMessage("Assigned system admin ID is required");
    }
}

