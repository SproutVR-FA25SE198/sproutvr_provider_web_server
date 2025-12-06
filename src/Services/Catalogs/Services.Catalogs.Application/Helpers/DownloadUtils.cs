namespace Services.Catalogs.Application.Helpers;

/// <summary>
/// Utility class for generating download paths from Google Drive URLs
/// </summary>
public static class DownloadUtils
{
    /// <summary>
    /// Converts Google Drive view URL to direct download URL
    /// Example: 
    /// Input:  https://drive.google.com/file/d/FILE_ID/view
    /// Output: https://drive.google.com/uc?export=download&id=FILE_ID
    /// </summary>
#pragma warning disable CA1054 // URI-like parameters should not be strings
    public static string GenerateDownloadPath(string? driveViewUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
    {
        if (string.IsNullOrWhiteSpace(driveViewUrl))
        {
            return string.Empty;
        }

        // Extract file ID from Google Drive URL
        // Format: https://drive.google.com/file/d/{FILE_ID}/view
        System.Text.RegularExpressions.Match fileIdMatch = System.Text.RegularExpressions.Regex.Match(
            driveViewUrl, 
            @"drive\.google\.com/file/d/([^/]+)");

        if (!fileIdMatch.Success)
        {
            // Return original if not a valid Google Drive URL
            return driveViewUrl;
        }

        string fileId = fileIdMatch.Groups[1].Value;
        
        // Generate direct download URL
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }
}
