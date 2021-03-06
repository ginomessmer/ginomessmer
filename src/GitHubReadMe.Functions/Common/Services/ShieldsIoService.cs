﻿using GitHubReadMe.Functions.Common.Data;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubReadMe.Functions.Common.Services
{
    public class ShieldsIoService : IShieldService
    {
        /// <summary>
        /// Retrieves a shield from shields.io.
        /// </summary>
        /// <param name="shield"></param>
        /// <returns></returns>
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
