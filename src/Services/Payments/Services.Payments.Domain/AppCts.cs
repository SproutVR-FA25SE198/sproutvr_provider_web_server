namespace Services.Payments.Domain;
public static class AppCts
{
    // Error Messages
    public static class Errors
    {
        // Payment Transactions
        public static class PaymentTransactions
        {
            public const string NotFound = "The Payment Transaction is not found. Please try again.";
            public const string AlreadyExists = "The Payment Transaction is already existed.";
        }
    }
    // Contains the directory of the file to seed the data
    public static class SeederFilePaths
    {
        // Get the folder at runtime
        private const string JsonFolderPath = "Data/SeederFiles";

        // Each json file path
        public static readonly string PaymentFilePath = Path.Combine(JsonFolderPath, "PaymentTransaction.json");
    }
}
