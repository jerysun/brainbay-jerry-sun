using FluentValidation;

namespace Characters.Application.Characters.AddCharacter;

public class AddCharacterCommandValidator : AbstractValidator<AddCharacterCommand>
{
    public AddCharacterCommandValidator()
    {
        RuleFor(x => x.CharacterDto.Name).NotEmpty().WithMessage("Name is required").Length(1,200);
        RuleFor(x => x.CharacterDto.Status).NotEmpty().WithMessage("Status is required").Length(1,50);
        RuleFor(x => x.CharacterDto.Species).NotEmpty().WithMessage("Species is required").Length(1,50);
        RuleFor(x => x.CharacterDto.Type).NotEmpty().WithMessage("Type is required").Length(1,100);
        RuleFor(x => x.CharacterDto.Gender).NotEmpty().WithMessage("Gender is required").Length(1,50);
        RuleFor(x => x.CharacterDto.Image).NotEmpty().WithMessage("Image is required").Length(1,500);
        RuleFor(x => x.CharacterDto.Url).NotEmpty().WithMessage("Url is required").Length(1,200);
        RuleFor(x => x.CharacterDto.Created).NotNull().WithMessage("Created is required");
    }
}