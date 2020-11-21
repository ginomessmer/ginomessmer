using GitHubReadMe.Functions.Common.Data;
using GitHubReadMe.Functions.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

            var result = new FileStreamResult(stream, ShieldDefaults.ContentType); 

            context.HttpContext.Response.Headers.Add("Cache-Control", ShieldDefaults.StaleCacheControl);
            await result.ExecuteResultAsync(context);
        }
    }
}
