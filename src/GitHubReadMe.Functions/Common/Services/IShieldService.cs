using System.IO;
using System.Threading.Tasks;
using GitHubReadMe.Functions.Data;

namespace GitHubReadMe.Functions.Common.Services
{
    public interface IShieldService
    {
        Task<Stream> GetShieldAsync(Shield shield);
    }
}