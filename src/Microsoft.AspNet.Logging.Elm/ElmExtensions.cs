using Microsoft.AspNet.Logging.Elm;

namespace Microsoft.AspNet.Builder
{
    public static class ElmExtensions
    {     
        public static IApplicationBuilder UseElm(this IApplicationBuilder builder)
        {
            return builder.UseElm(new ElmOptions());
        }

        public static IApplicationBuilder UseElm(this IApplicationBuilder builder, ElmOptions options)
        {
            return builder.UseMiddleware<ElmMiddleware>(options);
        }
    }
}