using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GitHubReadMe.Functions.Hits
{
    public class HitsBadge
    {
        private readonly IConfiguration _configuration;

        public HitsBadge(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("HitsBadge")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Table("GhRmHits")] CloudTable table,
            ILogger log)
        {
            await table.CreateIfNotExistsAsync();

            var insertOperation = TableOperation.Insert(new Hit());
            await table.ExecuteAsync(insertOperation);

            log.LogInformation("Hit recorded");

            // Retrieve current count
            var count = table.ExecuteQuery(new TableQuery()).Count();

            using (var httpClient = new HttpClient())
            {
                // Proxy response
                var stream = await httpClient.GetStreamAsync($"https://img.shields.io/badge/views-{count}-green");
                return new FileStreamResult(stream, "image/svg+xml");
            }
        }
    }
}
