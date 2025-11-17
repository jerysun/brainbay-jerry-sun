using Characters.Application.Data;
using Characters.Commons.MemoryCache;

namespace Characters.Application.Characters.AddCharacter;

public class AddCharacterCommandHandler(ICharacterContext dbContext, IMemoryCacheHelper memCache) : ICommandHandler<AddCharacterCommand, Character>
{
    public async Task<Character> Handle(AddCharacterCommand command, CancellationToken cancellationToken)
    {
        var character = command.CharacterDto.Adapt<Character>();
        dbContext.Characters.Add(character);
        await dbContext.SaveChangesAsync(cancellationToken);
        memCache.Remove("AllCharacters");

        return character;
    }
}