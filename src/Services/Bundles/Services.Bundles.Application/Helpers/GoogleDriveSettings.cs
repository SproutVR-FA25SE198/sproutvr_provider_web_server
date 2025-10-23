namespace Services.Bundles.Application.Helpers;
public class GoogleDriveSettings
{
    public string Email { get; set; }
    public string FolderId { get; set; } // parent folder of all customer bundle
    public string CredentialFolderPath { get; set; }
    public string CredentialFilePath { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RefreshToken { get; set; }

}
