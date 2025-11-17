using Characters.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Characters.Application.Data;

public interface ICharacterContext
{
    DbSet<Character> Characters { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}