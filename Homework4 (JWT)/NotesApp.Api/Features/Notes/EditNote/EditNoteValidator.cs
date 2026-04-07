using FluentValidation;

namespace NotesApp.Api.Features.Notes.EditNote;

public class EditNoteValidator : AbstractValidator<EditNoteRequest>
{
    public EditNoteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Content)
            .NotEmpty()
            .MinimumLength(5);
    }
}