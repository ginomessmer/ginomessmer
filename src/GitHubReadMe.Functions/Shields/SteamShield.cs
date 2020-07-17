using System;
using System.IO;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Options;
using GitHubReadMe.Functions.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SteamWebAPI2.Interfaces;

namespace GitHubReadMe.Functions.Shields
{
    public class SteamShield
    {
        private readonly ISteamUser _steamUser;
        private readonly SteamOptions _options;

        public SteamShield(ISteamUser steamUser, IOptions<SteamOptions> options)
        {
            _steamUser = steamUser;
            _options = options.Value;
        }

        [FunctionName("SteamShield")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "shields/steam")] HttpRequest req,
            ILogger log)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(_options.SteamId);
            var summary = response.Data;

            var result = summary.PlayingGameName != null
                ? new ShieldResult("now playing", summary.PlayingGameName, "success", "steam")
                : new ShieldResult("now playing", "n/a", logo: "steam");

            return result;
        }
    }
}
