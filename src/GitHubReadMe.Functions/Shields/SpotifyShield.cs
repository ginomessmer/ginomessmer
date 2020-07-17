using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Results;
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
        private readonly SpotifyOptions _options;

        public SpotifyShield(IOptions<SpotifyOptions> options)
        {
            _options = options.Value;
        }

        [FunctionName("SpotifyShield")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "shields/spotify")] HttpRequest req,
            ILogger log)
        {
            try
            {
                using var httpClient = new HttpClient()
                {
                    DefaultRequestHeaders =
                    {
                        {
                            "Authorization",
                            $"Bearer {_options.AccessToken}"
                        }
                    }
                };

                var json = await httpClient.GetStringAsync("https://api.spotify.com/v1/me/player/currently-playing");
                var doc = JsonDocument.Parse(json);

                var artist = doc.RootElement.GetProperty("item").GetProperty("artists")[0].GetProperty("name").GetString();
                var track = doc.RootElement.GetProperty("item").GetProperty("name").GetString();

                return new ShieldResult("spotify", $"{artist}: {track}", "green");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while fetching Spotify playback");
                return new ShieldResult("spotify", "n/a");
            }
        }
    }
}
