namespace Microservices.Banking.Core.Exceptions;

public sealed class DatabaseAcknowledgeException : CoreException
{
    public DatabaseAcknowledgeException(string message)
        : base(message)
    {
    }
}