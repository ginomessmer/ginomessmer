using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GitHubReadMe.Functions.Hits
{
    public static class SumHits
    {
        [FunctionName("SumHits")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("GhRmHits")] CloudTable table)
        {
            var countQuery = new TableQuery<Hit>();
            var count = table.ExecuteQuery(countQuery).Count();

            return new OkObjectResult(new
            {
                Total = count
            });
        }
    }
}
