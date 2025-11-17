using Characters.Commons.Pagination;
using Characters.Domain.Models;

namespace Characters.Application.Characters.GetCharacters;

public record GetCharactersResult(PaginatedResult<Character> Characters);