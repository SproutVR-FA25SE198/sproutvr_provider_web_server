namespace Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;
public class ApplicationUserDto
{

#pragma warning disable S125 // Sections of code should not be commented out
    //public Guid Id { get; set; }
#pragma warning restore S125 // Sections of code should not be commented out
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
