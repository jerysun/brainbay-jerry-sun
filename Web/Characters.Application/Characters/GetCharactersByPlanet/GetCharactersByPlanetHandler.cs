namespace Characters.Application.Characters.GetCharactersByPlanet;

public class GetCharactersByPlanetHandler(ICharacterContext dbContext, IMemoryCacheHelper memCache)
    : IQueryHandler<GetCharactersByPlanetQuery, GetCharactersByPlanetResult>
{
    public async Task<GetCharactersByPlanetResult> Handle(GetCharactersByPlanetQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        string planet = query.Planet;

        var totalCount = await dbContext.Characters.LongCountAsync(cancellationToken);

        var (characters, fromCache) = await memCache.GetOrCreateAsync("CharactersFrom" + planet, async (e) =>
            await dbContext.Characters
                .AsNoTracking()
                .Where(x => x.Origin.Name.Contains(planet))
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
        );

        return new GetCharactersByPlanetResult(
            new PaginatedResult<Character>(
                pageIndex,
                pageSize,
                totalCount,
                characters), fromCache);
    }
}