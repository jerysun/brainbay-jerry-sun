namespace Characters.Application.Characters.AddCharacter;

public class AddCharacterCommandHandler(ICharacterContext dbContext, IMemoryCacheHelper memCache) : ICommandHandler<AddCharacterCommand, Character>
{
    public async Task<Character> Handle(AddCharacterCommand command, CancellationToken cancellationToken)
    {
        var character = command.CharacterDto.Adapt<Character>();
        dbContext.Characters.Add(character);
        await dbContext.SaveChangesAsync(cancellationToken);
        memCache.ClearAll();

        return character;
    }
}