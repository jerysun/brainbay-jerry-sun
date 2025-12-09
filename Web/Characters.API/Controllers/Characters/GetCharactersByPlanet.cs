namespace Characters.API.Controllers.Characters;

public record GetCharactersByPlanetRequest(int PageIndex = 0, int PageSize = 20, string Planet = "Earth");

public record GetCharactersByPlanetResponse(PaginatedResult<Character> Characters);