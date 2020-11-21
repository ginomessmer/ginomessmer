using System.IO;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Data;

namespace GitHubReadMe.Functions.Common.Services
{
    public interface IShieldService
    {
        Task<Stream> GetShieldAsync(Shield shield);
    }

    public static class ShieldDefaults
    {
        public const string ContentType = "image/svg+xml";
        public const string StaleCacheControl = "s-maxage=1, stale-while-revalidate";
    }
}