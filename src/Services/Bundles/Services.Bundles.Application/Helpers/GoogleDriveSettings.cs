namespace Services.Bundles.Application.Helpers;
public class GoogleDriveSettings
{
    public string Email { get; set; }
    public string CustomerFolderId { get; set; } // parent folder of all customer bundle
    public string MetadataFolderId { get; set; }
    public string CredentialFolderPath { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RefreshToken { get; set; }
    public string RedirectUri { get; set; }

}
