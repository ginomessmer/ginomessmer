using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace GitHubReadMe.Functions.Common.Results
{
    public class ShieldResult : IActionResult
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public string Logo { get; set; }


        public ShieldResult(string title, string value, string color = "inactive", string logo = "")
        {
            Title = title;
            Value = value;
            Color = color;
            Logo = logo;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            using var httpClient = new HttpClient();
            context.HttpContext.Response.Headers.Add("Cache-Control", "s-maxage=1, stale-while-revalidate");

            var url = $"https://img.shields.io/badge/{Title}-{Value}-{Color}";
            url = QueryHelpers.AddQueryString(url, "logo", Logo);

            var stream = await httpClient.GetStreamAsync(url);
            var result = new FileStreamResult(stream, "image/svg+xml");

            await result.ExecuteResultAsync(context);
        }
    }
}
