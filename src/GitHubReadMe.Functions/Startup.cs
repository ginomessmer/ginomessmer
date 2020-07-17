using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions;
using GitHubReadMe.Functions.Common.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
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

            builder.Services.AddSingleton(new SecretClient(new Uri(configuration["KeyVault:VaultUri"]), 
                new ClientSecretCredential(configuration["KeyVault:TenantId"], 
                    configuration["KeyVault:ClientId"], 
                    configuration["KeyVault:ClientSecret"])));

            builder.Services.AddOptions();
            builder.Services.Configure<SpotifyOptions>(configuration.GetSection("Spotify"));

            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddOptions();
        }
    }
}
