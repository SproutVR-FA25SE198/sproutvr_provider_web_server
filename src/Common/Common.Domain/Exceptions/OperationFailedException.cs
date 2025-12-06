namespace Common.Domain.Exceptions;
public class OperationFailedException : Exception
{
    public string Operation { get; private set; }
    public string ResourceIdentifier { get; private set; }
    public OperationFailedException()
        : base("The requested operation failed.") { }

    public OperationFailedException(string message)
        : base(message) { }

}
