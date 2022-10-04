using FluentValidation;
using Microservices.Application.Models;

namespace Microservices.Application.Validators;

public class InitiateTransactionRequestValidator : AbstractValidator<InitiateTransactionRequest>
{
    public InitiateTransactionRequestValidator()
    {
        RuleFor(model => model.CardFrom).NotEmpty().MinimumLength(15).MaximumLength(19).Matches("^\\d+$");
        RuleFor(model => model.UsdAmount).GreaterThan(0).LessThan(100_000);
    }
}