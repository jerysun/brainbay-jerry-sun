namespace Characters.Application.Characters.GetCharacters;

public record GetCharactersResult(PaginatedResult<Character> Characters, bool FromCache);