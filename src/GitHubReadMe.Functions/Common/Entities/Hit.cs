using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHubReadMe.Functions.Common.Entities
{
    public class Hit : TableEntity
    {
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public Hit()
        {
            PartitionKey = nameof(Hit);
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
