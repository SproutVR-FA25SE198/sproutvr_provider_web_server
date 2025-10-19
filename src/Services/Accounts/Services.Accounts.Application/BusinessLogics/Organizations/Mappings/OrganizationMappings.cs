using Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
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

    public static OrganizationDetailsDto ToDetailsDto (this Organization org)
    {
        var dto = new OrganizationDetailsDto
        {
            //Id = org.Id,
            Email = org.Email,
            PhoneNumber = org.PhoneNumber,
            UserName = org.UserName,
            AvatarUrl = org.AvatarUrl,
            Status = org.Status.ToString(),
            CreatedAtUtc = org.CreatedAtUtc,
            UpdatedAtUtc = org.UpdatedAtUtc,
            Name = org.Name,
            Address = org.Address,
            MACAddress = org.MACAddress,
            ActivationKey = org.ActivationKey,
            BundleGoogleDriveUrl = org.BundleGoogleDriveUrl,
        };
        return dto;
    }

    public static OrganizationDto ToDto(this Organization org)
    {
        return new OrganizationDto
        {
            Id = org.Id,
            Email = org.Email,
            PhoneNumber = org.PhoneNumber,
            UserName = org.UserName,
            AvatarUrl = org.AvatarUrl,
            Status = org.Status.ToString(),
            CreatedAtUtc = org.CreatedAtUtc,
            UpdatedAtUtc = org.UpdatedAtUtc,
            Name = org.Name,
            Address = org.Address,
            MACAddress = org.MACAddress,
            ActivationKey = org.ActivationKey,
            BundleGoogleDriveUrl = org.BundleGoogleDriveUrl,
        };
    }
}
