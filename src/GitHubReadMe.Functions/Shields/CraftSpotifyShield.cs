using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Services;
using GitHubReadMe.Functions.Data;
using GitHubReadMe.Functions.Spotify;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GitHubReadMe.Functions.Shields
{
    public class CraftSpotifyShield
    {
        private readonly SecretClient _secretClient;
        private readonly IShieldService _shieldService;

        public CraftSpotifyShield(SecretClient secretClient, IShieldService shieldService)
        {
            _secretClient = secretClient;
            _shieldService = shieldService;
        }

        [FunctionName("CraftSpotifyShield")]
        public async Task Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("shields/spotify.svg", FileAccess.ReadWrite)] ICloudBlob spotifyShieldBlob, ILogger log)
        {
            var accessToken = (await _secretClient.GetSecretAsync(RefreshSpotifyAccessToken.SpotifyAccessTokenSecretName)).Value.Value;

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var json = await httpClient.GetStringAsync("https://api.spotify.com/v1/me/player/currently-playing");
                var jsonDocument = JsonDocument.Parse(json);

                var artist = jsonDocument.RootElement.GetProperty("item").GetProperty("artists")[0].GetProperty("name").GetString();
                var track = jsonDocument.RootElement.GetProperty("item").GetProperty("name").GetString();

                var stream = await _shieldService.GetShieldAsync(
                    new Shield("listening to", $"{artist}: {track}", "brightgreen", "spotify"));

                spotifyShieldBlob.Properties.ContentType = "image/svg+xml";
                spotifyShieldBlob.Properties.CacheControl = "s-maxage=1, stale-while-revalidate"; 
                await spotifyShieldBlob.UploadFromStreamAsync(stream);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while fetching Spotify playback");
            }
        }
    }
}
