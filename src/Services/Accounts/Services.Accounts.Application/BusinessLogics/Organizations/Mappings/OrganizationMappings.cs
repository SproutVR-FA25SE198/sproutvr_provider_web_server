using Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;
using Services.Accounts.Domain.Entities.Organizations;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Mappings;
public static class OrganizationMappings
{
    public static Organization ToEntity(CreateOrganizationCommand command)
    {
        var organization = new Organization()
        {
            Name = command.Name,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            Address = command.Address,
        };
        return organization;
    }

    public static Organization ToEntity(Organization org, UpdateOrganizationCommand command)
    {
        org.Name = command.Name ?? org.Name;
        org.Address = command.Address ?? org.Address;
        org.Email = command.Email ?? org.Email;
        org.PhoneNumber = command.PhoneNumber ?? org.PhoneNumber;
        org.MACAddress = command.MACAddress ?? org.MACAddress;
        org.BundleGoogleDriveUrl = command.BundleGoogleDriveUrl ?? org.BundleGoogleDriveUrl;
        return org;
    }
}
