using Microsoft.Azure.Cosmos.Table;
using System;

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
