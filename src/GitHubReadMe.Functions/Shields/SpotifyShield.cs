using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Results;
using GitHubReadMe.Functions.Spotify;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

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
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                var json = await httpClient.GetStringAsync("https://api.spotify.com/v1/me/player/currently-playing");
                var jsonDocument = JsonDocument.Parse(json);

                var artist = jsonDocument.RootElement.GetProperty("item").GetProperty("artists")[0].GetProperty("name").GetString();
                var track = jsonDocument.RootElement.GetProperty("item").GetProperty("name").GetString();

                return new ShieldResult("listening to", $"{artist} - {track}",
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
