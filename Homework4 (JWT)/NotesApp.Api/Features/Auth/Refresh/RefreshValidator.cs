using FluentValidation;

namespace NotesApp.Api.Features.Auth.Refresh;

public class RefreshValidator : AbstractValidator<RefreshRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}