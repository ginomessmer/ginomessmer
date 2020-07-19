using System.IO;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Services;
using GitHubReadMe.Functions.Data;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamWebAPI2.Interfaces;

namespace GitHubReadMe.Functions.Shields
{
    public class CraftSteamShield
    {
        private readonly ISteamUser _steamUser;
        private readonly IShieldService _shieldService;
        private readonly SteamOptions _options;

        public CraftSteamShield(ISteamUser steamUser, IShieldService shieldService, IOptions<SteamOptions> options)
        {
            _steamUser = steamUser;
            _shieldService = shieldService;
            _options = options.Value;
        }

        [FunctionName("CraftSteamShield")]
        public async Task Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer,
            [Blob("shields/steam.svg", FileAccess.ReadWrite)] ICloudBlob steamShieldBlob,
            ILogger log)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(_options.SteamId);
            var summary = response.Data;

            var shield = summary.PlayingGameName != null
                ? new Shield("now playing", summary.PlayingGameName, "success", "steam")
                : new Shield("now playing", "n/a", logo: "steam");

            var stream = await _shieldService.GetShieldAsync(shield);

            steamShieldBlob.Properties.ContentType = "image/svg+xml";
            steamShieldBlob.Properties.CacheControl = "s-maxage=1, stale-while-revalidate";
            await steamShieldBlob.UploadFromStreamAsync(stream);
        }
    }
}
