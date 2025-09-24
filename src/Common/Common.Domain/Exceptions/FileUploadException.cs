namespace Common.Domain.Exceptions;

public class FileUploadException : Exception
{
    public FileUploadException()
        : base("Unexpected error occur while performing file upload.") { }

    public FileUploadException(string message)
        : base(message) { }
}
