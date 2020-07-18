using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubReadMe.Functions.Common.Results
{
    public class ShieldResult : IActionResult
    {
        public string Title { get; set; }

        public string Value { get; set; }

        public string Color { get; set; }

        public string Logo { get; set; }

        public ShieldResult()
        {
        }

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

            var url = $"https://img.shields.io/badge/{SafeShieldsEscape(Title)}-{SafeShieldsEscape(Value)}-{Color}";
            url = QueryHelpers.AddQueryString(url, "logo", Logo);

            var stream = await httpClient.GetStreamAsync(url);
            var result = new FileStreamResult(stream, "image/svg+xml");

            await result.ExecuteResultAsync(context);
        }

        public static string SafeShieldsEscape(string input) => input
            .Replace("-", "--")
            .Replace("_", "__");
    }
}
