using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Bundles.Application.Helpers;

namespace Services.Bundles.Infrastructure.Helpers;
public class OAuthHelper
{
    private readonly GoogleDriveSettings _googleDriveSettings;
    private readonly ILogger<OAuthHelper> _logger;

    private readonly string[] _scopes = {
                                      DriveService.Scope.Drive,
                                      DriveService.Scope.DriveFile,
                                      DriveService.Scope.DriveMetadata
                                  };
    private const string _applicationName = "SproutVR";


    public OAuthHelper(IOptions<GoogleDriveSettings> googleDriveSettings, ILogger<OAuthHelper> logger)
    {
        _googleDriveSettings = googleDriveSettings.Value;
        _logger = logger;
    }

    public DriveService GetDriveService()
    {
        try
        {
            UserCredential credential = GetCredential();

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }
        catch (Exception ex)
        {
            if (ex.Message == "REFRESH_TOKEN_INVALID")
            {
                throw new Exception("GOOGLE_REAUTH_REQUIRED");
            }

            throw;
        }
    }


    private UserCredential GetCredential()
    {
        var token = new TokenResponse
        {
            RefreshToken = _googleDriveSettings.RefreshToken
        };

        using var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleDriveSettings.ClientId,
                ClientSecret = _googleDriveSettings.ClientSecret
            },
            Scopes = _scopes
        });

        var credential = new UserCredential(flow, _googleDriveSettings.Email, token);

        try
        {
            bool refreshed = credential.RefreshTokenAsync(CancellationToken.None).Result;

            if (refreshed && credential.Token.RefreshToken != null)
            {
                SaveRefreshToken(credential.Token.RefreshToken);
            }
        }
        catch (Exception ex)
        {
            // Google trả về invalid_grant khi refresh token hết hạn hoặc bị revoke
            if (ex.Message.Contains("invalid_grant"))
            {
                throw new Exception("REFRESH_TOKEN_INVALID");
            }

            throw;
        }

        return credential;
    }


#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    private void SaveRefreshToken(string newRefreshToken)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
    {
        _logger.LogInformation("Saving new refresh token: {NewRefreshToken}", newRefreshToken);

        File.WriteAllText("refresh_token.txt", newRefreshToken);
        _googleDriveSettings.RefreshToken = newRefreshToken;
    }

    // STEP 1: create link login google
#pragma warning disable CA1055 // URI-like return values should not be strings
    public string GetGoogleOAuthUrl()
#pragma warning restore CA1055 // URI-like return values should not be strings
    {
        string scope = string.Join(" ", _scopes);
        return $"https://accounts.google.com/o/oauth2/v2/auth?client_id={_googleDriveSettings.ClientId}&redirect_uri={_googleDriveSettings.RedirectUri}&response_type=code&access_type=offline&prompt=consent&scope={scope}";
    }

    // STEP 2: callback backend to get code => change token => store refresh token
    public async Task<string> ExchangeCodeForRefreshToken(string code)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleDriveSettings.ClientId,
                ClientSecret = _googleDriveSettings.ClientSecret
            },
            Scopes = _scopes
        });
#pragma warning restore CA2000 // Dispose objects before losing scope

        TokenResponse token = await flow.ExchangeCodeForTokenAsync(
            userId: "user",
            code: code,
            redirectUri: _googleDriveSettings.RedirectUri,
            taskCancellationToken: CancellationToken.None
        );

        if (!string.IsNullOrEmpty(token.RefreshToken))
        {
            SaveRefreshToken(token.RefreshToken);
        }

        return "OK";
    }

}

