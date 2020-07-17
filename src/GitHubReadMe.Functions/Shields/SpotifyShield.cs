using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Results;
using GitHubReadMe.Functions.Spotify;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GitHubReadMe.Functions.Shields
{
    public class SpotifyShield
    {
        private readonly SecretClient _secretClient;

        public SpotifyShield(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        [FunctionName("SpotifyShield")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "shields/spotify")] HttpRequest req,
            ILogger log)
        {
            var accessToken = (await _secretClient.GetSecretAsync(RefreshSpotifyAccessToken.SpotifyAccessTokenSecretName)).Value.Value;

            try
            {
                using var httpClient = new HttpClient
                {
                    DefaultRequestHeaders =
                    {
                        {
                            "Authorization", $"Bearer {accessToken}"
                        }
                    }
                };

                var json = await httpClient.GetStringAsync("https://api.spotify.com/v1/me/player/currently-playing");
                var jsonDocument = JsonDocument.Parse(json);

                var artist = jsonDocument.RootElement.GetProperty("item").GetProperty("artists")[0].GetProperty("name").GetString();
                var track = jsonDocument.RootElement.GetProperty("item").GetProperty("name").GetString();

                return new ShieldResult("listening to", $"{artist} -- {track}", 
                    "brightgreen", "spotify");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while fetching Spotify playback");
                return new ShieldResult("listening to", "n/a");
            }
        }
    }
}
