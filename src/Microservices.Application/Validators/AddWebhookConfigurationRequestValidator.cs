using FluentValidation;
using Microservices.Application.Models;

namespace Microservices.Application.Validators;

public sealed class AddWebhookConfigurationRequestValidator : AbstractValidator<AddWebhookConfigurationRequest>
{
    public AddWebhookConfigurationRequestValidator()
    {
        RuleFor(model => model.Url)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out Uri? uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Url is invalid.");

        RuleFor(model => model.Type).IsInEnum();
        RuleFor(model => model.Url).NotEmpty().MaximumLength(2048);
    }
}