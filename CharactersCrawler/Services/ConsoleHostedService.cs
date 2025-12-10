using CharactersCrawler.Data;
using CharactersCrawler.Dtos;
using CharactersCrawler.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CharactersCrawler.Services;

public class ConsoleHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConsoleHostedService> _logger;

    public ConsoleHostedService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider, ILogger<ConsoleHostedService> logger)
    {
        _appLifetime = appLifetime;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CharacterContext>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var characterLink = GetCharacterLink();

            context.Database.MigrateAsync(cancellationToken: cancellationToken).GetAwaiter().GetResult();
            await context.Characters.ExecuteDeleteAsync(cancellationToken);

            var characterDtos = await GetCharacterDtos(httpClientFactory, characterLink);
            var characters = characterDtos.Adapt<IEnumerable<Character>>();
            await context.Characters.AddRangeAsync(characters, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added the characters to database!");
        }

        _appLifetime.StopApplication();
    }

    private static string GetCharacterLink()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var characterLink = configuration.GetSection("CharacterLink").Value;
        return string.IsNullOrWhiteSpace(characterLink) ? "https://rickandmortyapi.com/api/character/" : characterLink;
    }

    private async Task<IEnumerable<CharacterDto>> GetCharacterDtos(IHttpClientFactory httpClientFactory,
        string characterLink)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var characterDtos = new List<CharacterDto>();

        try
        {
            var jsonResponse = await httpClient.GetStringAsync(characterLink);
            dynamic? response = JsonConvert.DeserializeObject<dynamic>(jsonResponse)!;
            int pages = (int)response.info.pages;
            _logger.LogInformation("Total pages: {Pages}", pages);
            string nextPage = response.info.next;
            nextPage = nextPage[0..^1];

            JArray? results = response.results;
            if (results != null)
            {
                var activeCharacters = await
                    GetActiveCharacters(results.ToObject<List<Character>>() ?? throw new InvalidOperationException());
                var activeCharacterDtos = activeCharacters.Adapt<List<CharacterDto>>();
                characterDtos.AddRange(activeCharacterDtos);
            }

            for (int i = 2; i <= pages; ++i)
            {
                string nextUrl = nextPage + i;
                jsonResponse = await httpClient.GetStringAsync(nextUrl);
                response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                if (response == null) continue;
                results = response.results;
                var loopCharacters = results.ToObject<List<Character>>();
                if (loopCharacters == null) continue;
                loopCharacters = await GetActiveCharacters(loopCharacters);
                var loopCharacterDtos = loopCharacters.Adapt<List<CharacterDto>>();
                characterDtos.AddRange(loopCharacterDtos);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("\n** ERROR: Could not access the API. **");
            _logger.LogError("Details: {exMessage}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred: {exMessage}", ex.StackTrace);
        }

        return characterDtos;
    }

    private static Task<List<Character>> GetActiveCharacters(List<Character> characters)
    {
        var activeCharacters =  characters.Where(c => c.Status == "Alive").ToList();
        return Task.FromResult(activeCharacters);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application stopping...");
        return Task.CompletedTask;
    }
}