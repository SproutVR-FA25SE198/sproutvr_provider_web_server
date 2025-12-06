namespace Common.Domain.Exceptions;

public class NotFoundException : Exception
{
    public string ResourceType { get; private set; }
    public string ResourceIdentifier { get; private set; }
    public NotFoundException()
        : base("The requested resource was not found.") { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string resourceType, string resourceIdentifier)
            : base($"{resourceType} with id: {resourceIdentifier} does not exists")
    {
        ResourceIdentifier = resourceIdentifier;
        ResourceType = resourceType;
    }

}
