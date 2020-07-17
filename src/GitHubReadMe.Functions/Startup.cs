using System;
using GitHubReadMe.Functions;
using GitHubReadMe.Functions.Common.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace GitHubReadMe.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true)
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets(GetType().Assembly, true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddOptions();
            builder.Services.Configure<SpotifyOptions>(configuration.GetSection("Spotify"));

            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddOptions();
        }
    }
}
