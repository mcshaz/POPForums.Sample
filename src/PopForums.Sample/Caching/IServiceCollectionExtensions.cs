using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PopForums.Sample.Caching
{
    public static class IServiceCollectionExtensions
    {

        public static void AddUrlHelper(this IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            services.TryAddScoped<IUrlHelper>(
                factory => factory.GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(factory.GetRequiredService<IActionContextAccessor>().ActionContext)
            );
        }

    }
}
