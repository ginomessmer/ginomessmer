using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Services;
using GitHubReadMe.Functions.Data;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GitHubReadMe.Functions.Shields
{
    /// <summary>
    /// Creates a shield that displays my stats from Microsoft Learn.
    /// </summary>
    public class CraftMicrosoftLearnShield
    {
        private readonly IShieldService _shieldService;
        private readonly MicrosoftDocsOptions _options;

        public CraftMicrosoftLearnShield(IOptions<MicrosoftDocsOptions> options, IShieldService shieldService)
        {
            _shieldService = shieldService;
            _options = options.Value;
        }

        /// <inheritdoc cref="CraftMicrosoftLearnShield"/>
        [FunctionName("CraftMicrosoftLearnShield")]
        public async Task Run([TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo myTimer,
            [Blob("shields/ms_learn.svg", FileAccess.ReadWrite)] ICloudBlob msLearnShieldBlob, ILogger log)
        {
            Stream stream = new MemoryStream();

            try
            {
                using var httpClient = new HttpClient();
                var json = await httpClient.GetStringAsync($"https://docs.microsoft.com/api/gamestatus/{_options.GameStatusId}");

                var jsonDocument = JsonDocument.Parse(json);
                var rootElement = jsonDocument.RootElement;
                var levelElement = rootElement.GetProperty("level");

                var levelNumber = levelElement.GetProperty("levelNumber").GetInt32();
                var pointsHigh = levelElement.GetProperty("pointsHigh").GetInt32();
                var totalPoints = rootElement.GetProperty("totalPoints").GetInt32();


                var shieldValue = $"Level {levelNumber} ({totalPoints}/{pointsHigh} XP)";
                stream = await _shieldService.GetShieldAsync(new Shield("Microsoft Learn", shieldValue,
                    ShieldDefaults.GreenColor, "microsoft"));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while fetching from MS Docs");
                return;
            }

            msLearnShieldBlob.Properties.ContentType = ShieldDefaults.ContentType;
            msLearnShieldBlob.Properties.CacheControl = ShieldDefaults.StaleCacheControl;
            await msLearnShieldBlob.UploadFromStreamAsync(stream);
        }
    }
}
