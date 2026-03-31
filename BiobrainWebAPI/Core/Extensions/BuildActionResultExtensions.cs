using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BiobrainWebAPI.Core.Extensions
{
    public static class BuildActionResultExtensions
    {
        public static async Task<ActionResult<T>> ToActionResult<T>(this Task<T> task) => new OkObjectResult(await task);
    }
}
