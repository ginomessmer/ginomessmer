using Azure.Security.KeyVault.Secrets;
using GitHubReadMe.Functions.Common.Options;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubReadMe.Functions.Spotify
{
    /// <summary>
    /// Refreshes the Spotify access token every few minutes and stores it to Key Vault.
    /// </summary>
    public class RefreshSpotifyAccessToken
    {
        public const string SpotifyAccessTokenSecretName = "Spotify--AccessToken";

        private readonly SecretClient _secretClient;
        private SpotifyOptions _options;

        public RefreshSpotifyAccessToken(IOptions<SpotifyOptions> options, SecretClient secretClient)
        {
            _secretClient = secretClient;
            _options = options.Value;
        }

        [FunctionName(nameof(RefreshSpotifyAccessToken))]
        public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo timer,
            ILogger log)
        {
            // Generate basic auth header value
            var basicAuthHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

            // Prepare request
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", _options.RefreshToken)
                })
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Get access token
            var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var accessToken = jsonDocument.RootElement.GetProperty("access_token").GetString();

            // Save in vault
            await _secretClient.SetSecretAsync(SpotifyAccessTokenSecretName, accessToken);
            log.LogInformation("Spotify access token refreshed");
        }
    }
}
