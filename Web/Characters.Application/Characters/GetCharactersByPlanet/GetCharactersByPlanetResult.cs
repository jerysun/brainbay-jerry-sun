namespace Characters.Application.Characters.GetCharactersByPlanet;

public record GetCharactersByPlanetResult(PaginatedResult<Character> Characters, bool FromCache);