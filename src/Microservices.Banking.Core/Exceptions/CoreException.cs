namespace Microservices.Banking.Core.Exceptions;

public class CoreException : Exception
{
    protected CoreException(string message)
        : base(message)
    {
        PropertyErrors = Array.Empty<PropertyErrors>();
    }

    protected CoreException(IEnumerable<PropertyErrors> propertyErrors)
        : base("Request is invalid.")
    {
        PropertyErrors = propertyErrors.ToArray();
    }

    public PropertyErrors[] PropertyErrors { get; }
}