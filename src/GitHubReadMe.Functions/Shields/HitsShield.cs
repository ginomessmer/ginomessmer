using System.Linq;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Entities;
using GitHubReadMe.Functions.Common.Results;
using GitHubReadMe.Functions.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GitHubReadMe.Functions.Shields
{
    /// <summary>
    /// Creates a 'total views' shield.
    /// </summary>
    public class HitsShield
    {
        private readonly IConfiguration _configuration;

        public HitsShield(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc cref="HitsShield"/>
        [FunctionName("HitsShield")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "shields/hits")] HttpRequest req,
            [Table("GhRmHits")] CloudTable table,
            ILogger log)
        {
            await table.CreateIfNotExistsAsync();

            // Insert new hit
            var insertOperation = TableOperation.Insert(new Hit());
            await table.ExecuteAsync(insertOperation);

            log.LogInformation("Hit recorded");

            // Retrieve current count
            var count = table.ExecuteQuery(new TableQuery()).Count();

            return new ShieldResult("views", count.ToString(), ShieldDefaults.BlueColor);
        }
    }
}
