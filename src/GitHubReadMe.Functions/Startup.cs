using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions;
using GitHubReadMe.Functions.Common.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using GitHubReadMe.Functions.Common.Services;

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

            // Options
            builder.Services.AddOptions();
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.Configure<SpotifyOptions>(configuration.GetSection("Spotify"));
            builder.Services.Configure<SteamOptions>(configuration.GetSection("Steam"));

            // Key vault
            builder.Services.AddSingleton(new SecretClient(new Uri(configuration["KeyVault:VaultUri"]),
                new ClientSecretCredential(configuration["KeyVault:TenantId"],
                    configuration["KeyVault:ClientId"],
                    configuration["KeyVault:ClientSecret"])));

            // Shields
            builder.Services.AddSingleton<IShieldService, ShieldsIoService>();

            // Steam
            builder.Services.AddSingleton<ISteamUser>(new SteamWebInterfaceFactory(configuration["Steam:WebApiKey"])
                .CreateSteamWebInterface<SteamUser>(new HttpClient()));
        }
    }
}
