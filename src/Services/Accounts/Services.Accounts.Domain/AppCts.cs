namespace Services.Accounts.Domain;
public static class AppCts
{
    // Error Messages
    public static class Errors
    {
        // Accounts
        public static class ApplicationUsers
        {
            public const string NotFound = "The ApplicationUser is not found. Please try again.";
            public const string AlreadyExists = "The ApplicationUser is already existed.";
        }
    }

    // Contains the directory of the file to seed the data
    public static class SeederFilePaths
    {
        // Get the folder at runtime
        private const string JsonFolderPath = "Data/SeederFiles";

        // Each json file path
        public static readonly string OrganizationRegisterRequestFilePath = Path.Combine(JsonFolderPath, "OrganizationRegisterRequests.json");
    }

    public static class Roles
    {
        public const string SystemAdmin = "SystemAdmin";
        public const string Organization = "Organization";
    }
}
