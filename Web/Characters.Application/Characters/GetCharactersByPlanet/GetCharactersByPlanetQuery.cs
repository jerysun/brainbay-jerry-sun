namespace Characters.Application.Characters.GetCharactersByPlanet;

public record GetCharactersByPlanetQuery(PaginationRequest PaginationRequest, string Planet = "Earth") : IQuery<GetCharactersByPlanetResult>;