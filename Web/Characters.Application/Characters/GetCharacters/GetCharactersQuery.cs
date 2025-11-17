using Characters.Commons.Pagination;
using Characters.Commons.CQRS;

namespace Characters.Application.Characters.GetCharacters;

public record GetCharactersQuery(PaginationRequest PaginationRequest) : IQuery<GetCharactersResult>;