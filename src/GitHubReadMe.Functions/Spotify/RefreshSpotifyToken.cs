using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace GitHubReadMe.Functions.Spotify
{
    public class RefreshSpotifyToken
    {
        private readonly SecretClient _secretClient;
        private SpotifyOptions _options;

        public RefreshSpotifyToken(IOptions<SpotifyOptions> options, SecretClient secretClient)
        {
            _secretClient = secretClient;
            _options = options.Value;
        }

        [FunctionName("RefreshSpotifyToken")]
        public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo timer,
            ILogger log)
        {
            var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://accounts.spotify.com/api/token")
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", _options.RefreshToken)
                    }),
                    Headers =
                    {
                        { HeaderNames.Authorization, $"Basic {basicAuthValue}" }
                    }
                };

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                var accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();
                
                // Save in vault
                await _secretClient.SetSecretAsync("Spotify--AccessToken", accessToken);
                log.LogInformation("Spotify access token refreshed");
            }
        }
    }
}
