namespace Microservices.Banking.Core.Exceptions;

public sealed class AppValidationException : CoreException
{
    public AppValidationException(string propertyName, string error)
        : base(new []{ new PropertyErrors(propertyName, error) })
    {
    }
}