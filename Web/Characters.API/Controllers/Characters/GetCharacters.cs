namespace Characters.API.Controllers.Characters;

public record GetCharactersResponse(PaginatedResult<Character> Characters);