namespace Characters.Application.Characters.GetCharacters;

public class GetCharactersHandler(ICharacterContext dbContext, IMemoryCacheHelper memCache)
    : IQueryHandler<GetCharactersQuery, GetCharactersResult>
{
    public async Task<GetCharactersResult> Handle(GetCharactersQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await dbContext.Characters.LongCountAsync(cancellationToken);

        var (characters, fromCache) = await memCache.GetOrCreateAsync("AllCharacters", async (e) =>
                await dbContext.Characters
                    .AsNoTracking()
                    .OrderBy(c => c.Id)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken)
                );

        return new GetCharactersResult(
            new PaginatedResult<Character>(
                pageIndex,
                pageSize,
                totalCount,
                characters), fromCache);
    }
}