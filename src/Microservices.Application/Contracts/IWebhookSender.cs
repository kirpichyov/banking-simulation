namespace Microservices.Application.Contracts;

public interface IWebhookSender
{
    Task Send(string url, Guid secret, object data);
}