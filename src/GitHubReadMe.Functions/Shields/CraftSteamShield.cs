using GitHubReadMe.Functions.Common.Data;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Services;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SteamWebAPI2.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace GitHubReadMe.Functions.Shields
{
    /// <summary>
    /// Creates a shield that shows the game I'm currently playing.
    /// </summary>
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

        /// <inheritdoc cref="CraftSteamShield"/>
        [FunctionName("CraftSteamShield")]
        public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("shields/steam.svg", FileAccess.ReadWrite)] ICloudBlob steamShieldBlob,
            ILogger log)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(_options.SteamId);
            var summary = response.Data;

            var shield = summary.PlayingGameName != null
                ? new Shield("now playing", summary.PlayingGameName, ShieldDefaults.GreenColor, ShieldDefaults.SteamLogo)
                : new Shield("now playing", "n/a", logo: "steam");

            var stream = await _shieldService.GetShieldAsync(shield);

            steamShieldBlob.Properties.ContentType = ShieldDefaults.ContentType;
            steamShieldBlob.Properties.CacheControl = ShieldDefaults.StaleCacheControl;
            await steamShieldBlob.UploadFromStreamAsync(stream);
        }
    }
}
