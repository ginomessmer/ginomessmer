using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Data;
using Microsoft.AspNetCore.WebUtilities;

namespace GitHubReadMe.Functions.Common.Services
{
    public class ShieldsIoService : IShieldService
    {
        public async Task<Stream> GetShieldAsync(Shield shield)
        {
            using var httpClient = new HttpClient();

            var url = $"https://img.shields.io/badge/{SafeShieldsEscape(shield.Title)}-{SafeShieldsEscape(shield.Value)}-{shield.Color}";
            url = QueryHelpers.AddQueryString(url, "logo", shield.Logo);

            var stream = await httpClient.GetStreamAsync(url);
            return stream;
        }

        public static string SafeShieldsEscape(string input) => input
            .Replace("-", "--")
            .Replace("_", "__");
    }
}
