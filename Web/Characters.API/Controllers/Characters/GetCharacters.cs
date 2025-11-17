using Characters.Commons.Pagination;
using Characters.Domain.Models;

namespace Characters.API.Controllers.Characters;

public record GetCharactersResponse(PaginatedResult<Character> Characters);