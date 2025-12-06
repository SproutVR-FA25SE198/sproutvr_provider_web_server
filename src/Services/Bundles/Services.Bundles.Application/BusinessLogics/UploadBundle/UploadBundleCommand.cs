using MediatR;
using Microsoft.AspNetCore.Http;

namespace Services.Bundles.Application.BusinessLogics.UploadBundle;

public class UploadBundleCommand : IRequest
{
    public IFormFile BundleFile { get; set; }
    public OrderDto OrderDto { get; set; }
}
