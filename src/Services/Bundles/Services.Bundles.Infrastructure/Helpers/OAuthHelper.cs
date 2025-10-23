using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using Services.Bundles.Application.Helpers;

namespace Services.Bundles.Infrastructure.Helpers;
public class OAuthHelper
{
    private readonly GoogleDriveSettings _googleDriveSettings;

    private readonly string[] Scopes = {
                                      DriveService.Scope.Drive,
                                      DriveService.Scope.DriveFile,
                                      DriveService.Scope.DriveMetadata
                                  };
    private const string ApplicationName = "SproutVR";


    public OAuthHelper(IOptions<GoogleDriveSettings> googleDriveSettings)
    {
        _googleDriveSettings = googleDriveSettings.Value;
    }

    public DriveService GetInitAuthLocal()
    {
        string CredentialFolderPath = _googleDriveSettings.CredentialFolderPath;
        string CredentialFilePath = _googleDriveSettings.CredentialFilePath;
        using var stream = new FileStream(CredentialFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        Task<UserCredential> credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(CredentialFolderPath, true));
        UserCredential userCredential = credentials.Result;
        if (credentials.IsCanceled || credentials.IsFaulted)
        {
            throw new Exception("cannot connect");
        }

        var driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = userCredential,
            ApplicationName = ApplicationName,
        });
        return driveService;
    }

    public DriveService GetAuthCloud()
    {
        var tokenResponse = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
        {
            RefreshToken = _googleDriveSettings.RefreshToken
        };

        #pragma warning disable CA2000 // Dispose objects before losing scope
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleDriveSettings.ClientId,
                ClientSecret = _googleDriveSettings.ClientSecret
            },
            Scopes = Scopes
        });
        #pragma warning restore CA2000 // Dispose objects before losing scope

        var credential = new UserCredential(flow, _googleDriveSettings.Email, tokenResponse);

        // Auto refresh token when needed
        credential.RefreshTokenAsync(CancellationToken.None).Wait();

        var driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "SproutVR Drive Service"
        });

        return driveService;
    }

}

