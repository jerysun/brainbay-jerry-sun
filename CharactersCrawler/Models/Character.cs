using System.ComponentModel.DataAnnotations.Schema;

namespace CharactersCrawler.Models;

public class Character
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required string Species { get; init; }
    public required string Type { get; init; }
    public required string Gender { get; init; }
    public required Origin Origin { get; init; }
    public required Location Location { get; init; }
    public required string Image { get; init; }
    public required string[] Episode { get; init; }
    public required string Url { get; init; }
    public required DateTime Created { get; init; }
}