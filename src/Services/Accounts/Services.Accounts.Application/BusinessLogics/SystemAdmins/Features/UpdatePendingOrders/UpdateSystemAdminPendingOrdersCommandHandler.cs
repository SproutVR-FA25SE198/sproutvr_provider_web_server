using MediatR;
using Services.Accounts.Application.Abstractions.Data.Repositories;
using Services.Accounts.Domain.Entities.SystemAdmins;

namespace Services.Accounts.Application.BusinessLogics.SystemAdmins.Features.UpdatePendingOrders;
public class UpdateSystemAdminPendingOrdersCommandHandler : IRequestHandler<UpdateSystemAdminPendingOrdersCommand, bool>
{
    private readonly ISystemAdminRepository _systemAdminRepository;
    public UpdateSystemAdminPendingOrdersCommandHandler(ISystemAdminRepository systemAdminRepository)
    {
        _systemAdminRepository = systemAdminRepository;
    }
    public async Task<bool> Handle(UpdateSystemAdminPendingOrdersCommand request, CancellationToken cancellationToken)
    {
        SystemAdmin? systemAdmin = await _systemAdminRepository.GetByIdAsync(request.SystemAdminId);
        if (systemAdmin == null)
        {
            return false;
        }
        if (request.IsIncrement)
        {
            systemAdmin.NumberOfPendingOrders += 1;
        }
        else
        {
            systemAdmin.NumberOfPendingOrders = Math.Max(0, systemAdmin.NumberOfPendingOrders - 1);
        }
        _systemAdminRepository.Update(systemAdmin);
        await _systemAdminRepository.SaveChangesAsync();
        return true;
    }
}
