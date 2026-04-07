using FluentValidation;

namespace NotesApp.Api.Features.Notes.CreateNote;

public class CreateNoteValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Content)
            .NotEmpty()
            .MinimumLength(5);
    }
}