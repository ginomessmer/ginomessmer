using System;
using System.IO;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace GitHubReadMe.Functions.Hits
{
    public class RecordHit
    {
        private readonly IConfiguration _configuration;

        public RecordHit(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("RecordHit")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var connectionString = _configuration.GetConnectionString("DefaultStorageAccount");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference("hits");
            
            await table.CreateIfNotExistsAsync();

            var insertOperation = TableOperation.Insert(new Hit());
            await table.ExecuteAsync(insertOperation);

            log.LogInformation("Hit recorded");

            // TODO: Return badge with hit count
            return new OkResult();
        }
    }
}
