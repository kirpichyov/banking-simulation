namespace Microservices.Banking.Core.Exceptions;

public sealed class ItemNotFoundException : CoreException
{
    public ItemNotFoundException(string message)
        : base(message)
    {
        
    }
}