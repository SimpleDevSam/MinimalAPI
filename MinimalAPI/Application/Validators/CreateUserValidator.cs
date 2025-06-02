using FluentValidation;
using MinimalAPI_KeyCloack.Application.Requests;

namespace MinimalAPI_KeyCloack.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.UserEmail)
            .NotEmpty().WithMessage("User email is required")   
            .EmailAddress().WithMessage("Invalid email address");
        RuleFor(user => user.UserName).NotEmpty().WithMessage("User name is required");
    }
}