using MediatR;

namespace Services.Bundles.Application.BusinessLogics.CopyMetadataToOrder;

public class CopyMetadataToOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public string BundleGoogleDriveFolderId { get; set; } = string.Empty;
}

