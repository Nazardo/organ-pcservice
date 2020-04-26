using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService
{
    sealed class ShutdownProviderFactory : IFactory<IShutdownProvider>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly string implementation;

        public ShutdownProviderFactory(
            IServiceProvider serviceProvider,
            IOptions<FactoriesConfiguration> options)
        {
            this.serviceProvider = serviceProvider;
            implementation = options.Value.ShutdownProvider;
        }

        public IShutdownProvider Create()
        {
            if (implementation.Equals("win32", StringComparison.InvariantCultureIgnoreCase))
            {
                return serviceProvider.GetRequiredService<Win32.Win32Shutdown>();
            }
            return new DoNotShutdown();
        }
    }

    sealed class DoNotShutdown : IShutdownProvider
    {
        public void Shutdown() {}
    }
}