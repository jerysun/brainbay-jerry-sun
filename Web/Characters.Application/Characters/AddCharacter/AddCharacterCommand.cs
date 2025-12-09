namespace Characters.Application.Characters.AddCharacter;

public record AddCharacterCommand(CharacterDto CharacterDto) : ICommand<Character>;