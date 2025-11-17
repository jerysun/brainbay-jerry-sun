using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CharactersCrawler.Data;
using CharactersCrawler.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<CharacterContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

        services.AddHttpClient();
        services.AddHostedService<ConsoleHostedService>();
    })
    .Build();

await host.RunAsync();