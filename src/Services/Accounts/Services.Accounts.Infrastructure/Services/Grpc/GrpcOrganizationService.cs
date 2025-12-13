using Grpc.Core;
using MediatR;
using OrganizationAccountsService;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;

namespace Services.Accounts.Infrastructure.Services.Grpc;
public class GrpcOrganizationService : GrpcOrganization.GrpcOrganizationBase
{
    private readonly IMediator _mediator;
    public GrpcOrganizationService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<GetOrganizationByIdResponse> GetOrganizationById(GetOrganizationByIdRequest request, ServerCallContext context)
    {
        OrganizationDetailsDto queryResponse = await _mediator.Send(new GetOrganizationByIdQuery(Guid.Parse(request.OranganizationId)));

        return new GetOrganizationByIdResponse() 
        { 
            OrganizationEmail = queryResponse.Email, 
            OrganizationName = queryResponse.Name, 
            OrganizationPhoneNumber = queryResponse.PhoneNumber,
            BundleGoogleDriveId = queryResponse.BundleGoogleDriveId ?? string.Empty
        };

    }
}
