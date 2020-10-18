using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Tweetbook.Extensions
{
    public static class GeneralExtension
    {

        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User == null ? string.Empty : httpContext.User.Claims.Single(c => c.Type == "id").Value;
        }
    }
}