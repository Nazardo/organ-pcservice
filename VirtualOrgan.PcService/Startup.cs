using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VirtualOrgan.PcService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .Configure<FactoriesConfiguration>(Configuration.GetSection("factories"))
                .Configure<Archive.MidiArchiveConfiguration>(Configuration.GetSection("archive"))
                .Configure<Hauptwerk.HauptwerkConfiguration>(Configuration.GetSection("hauptwerk"))
                .Configure<Audio.MotuAvbConfiguration>(Configuration.GetSection("motu"))
                .Configure<RestartHauptwerkConfiguration>(Configuration.GetSection("restart"))
                .Configure<QuitAndShutdownConfiguration>(Configuration.GetSection("shutdown"))
                // Shutdown
                .AddSingleton<Win32.Win32Shutdown>()
                .AddSingleton<IFactory<IShutdownProvider>, ShutdownProviderFactory>()
                .AddSingleton<IShutdownProvider>(p => p.GetRequiredService<IFactory<IShutdownProvider>>().Create())
                // Audio Card
                .AddSingleton<Audio.MotuAvbAudioCard>()
                .AddSingleton<IFactory<Audio.IAudioCard>, Audio.AudioCardFactory>()
                .AddSingleton<Audio.IAudioCard>(p => p.GetRequiredService<IFactory<Audio.IAudioCard>>().Create())
                // Hauptwerk
                .AddSingleton<Hauptwerk.IProcessHelper, Hauptwerk.HauptwerkExeHelper>()
                .AddAlwaysOnSingleton<Hauptwerk.IHauptwerkMidiInterface, Hauptwerk.HauptwerkMidiInterface>()
                // Midi
                .AddSingleton<Archive.IMidiArchiveHandler, Archive.MidiArchiveHandler>()
                .AddSingleton<Midi.IMidiMessageSerializer, Midi.Impl.MessageSerializer>()
                .AddSingleton<Midi.IMidiInterface, Midi.Impl.MidiInterface>()

                .AddSingleton<ITaskFactory, QuitAndShutdownTaskFactory>()
                .AddSingleton<ITaskFactory, RestartHauptwerkTaskFactory>()

                .AddAlwaysOnSingleton<IPcService, PcService>()
                .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .ActivateAlwaysOnServices()
                .Run(NotFound);
        }

        private static Task NotFound(HttpContext context)
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }
    }
}
