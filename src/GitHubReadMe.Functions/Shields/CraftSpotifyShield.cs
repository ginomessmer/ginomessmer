using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Data;
using GitHubReadMe.Functions.Common.Services;
using GitHubReadMe.Functions.Spotify;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubReadMe.Functions.Shields
{
    /// <summary>
    /// Creates a shield that displays what I'm listening on Spotify right now.
    /// </summary>
    public class CraftSpotifyShield
    {
        private readonly SecretClient _secretClient;
        private readonly IShieldService _shieldService;

        public CraftSpotifyShield(SecretClient secretClient, IShieldService shieldService)
        {
            _secretClient = secretClient;
            _shieldService = shieldService;
        }

        /// <inheritdoc cref="CraftSpotifyShield"/>
        [FunctionName("CraftSpotifyShield")]
        public async Task Run([TimerTrigger("0 */4 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("shields/spotify.svg", FileAccess.ReadWrite)] ICloudBlob spotifyShieldBlob, ILogger log)
        {
            var accessToken = (await _secretClient.GetSecretAsync(RefreshSpotifyAccessToken.SpotifyAccessTokenSecretName)).Value.Value;
            Stream stream = new MemoryStream();

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var json = await httpClient.GetStringAsync("https://api.spotify.com/v1/me/player/currently-playing");
                var jsonDocument = JsonDocument.Parse(json);

                var artist = jsonDocument.RootElement.GetProperty("item").GetProperty("artists")[0].GetProperty("name").GetString();
                var track = jsonDocument.RootElement.GetProperty("item").GetProperty("name").GetString();

                stream = await _shieldService.GetShieldAsync(
                    new Shield("listening to", $"{artist}: {track}", ShieldDefaults.BrightGreenColor, "spotify"));

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while fetching Spotify playback");

                stream = await _shieldService.GetShieldAsync(
                    new Shield("listening to", $"n/a", ShieldDefaults.GreyColor, ShieldDefaults.SpotifyLogo));
            }

            spotifyShieldBlob.Properties.ContentType = ShieldDefaults.ContentType;
            spotifyShieldBlob.Properties.CacheControl = ShieldDefaults.StaleCacheControl;
            await spotifyShieldBlob.UploadFromStreamAsync(stream);
        }
    }
}
