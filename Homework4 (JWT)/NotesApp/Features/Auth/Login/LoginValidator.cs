using FluentValidation;

namespace NotesApp.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Username is required.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}