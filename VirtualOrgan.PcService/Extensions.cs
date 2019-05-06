using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VirtualOrgan.PcService
{
    internal static class Extensions
    {
        public static IServiceCollection AddAlwaysOnSingleton<TInterface, TImplementation>(this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.TryAddSingleton<TImplementation>();
            if (!services.Any(descriptor => descriptor.ImplementationType == typeof(AlwaysOnService<TImplementation>)))
            {
                services.AddTransient<AlwaysOnService, AlwaysOnService<TImplementation>>();
            }
            return services.AddSingleton<TInterface>(provider => provider.GetRequiredService<TImplementation>());
        }

        public static IApplicationBuilder ActivateAlwaysOnServices(this IApplicationBuilder app)
        {
            ActivatorUtilities.CreateInstance<AlwaysOnActivator>(app.ApplicationServices);
            return app;
        }

        private abstract class AlwaysOnService { }

        private class AlwaysOnService<TService> : AlwaysOnService
        {
            public AlwaysOnService(TService required) { }
        }

        private sealed class AlwaysOnActivator
        {
            public AlwaysOnActivator(IEnumerable<AlwaysOnService> allServices) { }
        }
    }
}
