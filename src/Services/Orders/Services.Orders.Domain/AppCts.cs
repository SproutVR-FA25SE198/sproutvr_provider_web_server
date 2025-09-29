namespace Services.Orders.Domain;

public static class AppCts
{
    // Error Messages
    public static class Errors
    {
        // Maps
        public static class Maps
        {
            public const string NotFound = "The Map is not found. Please try again.";
            public const string AlreadyExists = "The Map is already existed.";
        }
    }
    // Contains the directory of the file to seed the data
    public static class SeederFilePaths
    {
        // Get the folder at runtime
        private const string JsonFolderPath = "Data/SeederFiles";

        // Each json file path
        public static readonly string OrderFilePath = Path.Combine(JsonFolderPath, "Order.json");
        public static readonly string OrderItemFilePath = Path.Combine(JsonFolderPath, "OrderItem.json");
    }
}
