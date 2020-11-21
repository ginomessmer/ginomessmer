using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Common.Data;
using GitHubReadMe.Functions.Common.Services;

namespace GitHubReadMe.Functions.Common.Results
{
    public class ShieldResult : IActionResult
    {
        public Shield Shield { get; set; }

        public ShieldResult()
        {
        }

        public ShieldResult(Shield shield)
        {
            Shield = shield;
        }

        public ShieldResult(string title, string value, string color = "inactive", string logo = "") : this(new Shield(title, value, color, logo))
        {
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var service = new ShieldsIoService();
            var stream = await service.GetShieldAsync(Shield);

            var result = new FileStreamResult(stream, "image/svg+xml"); 

            context.HttpContext.Response.Headers.Add("Cache-Control", "s-maxage=1, stale-while-revalidate");
            await result.ExecuteResultAsync(context);
        }
    }
}
