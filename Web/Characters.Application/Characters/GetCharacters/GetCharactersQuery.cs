namespace Characters.Application.Characters.GetCharacters;

public record GetCharactersQuery(PaginationRequest PaginationRequest) : IQuery<GetCharactersResult>;