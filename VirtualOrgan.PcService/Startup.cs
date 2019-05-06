using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                .Configure<Archive.MidiArchiveConfiguration>(Configuration.GetSection("archive"))
                .Configure<Hauptwerk.HauptwerkConfiguration>(Configuration.GetSection("hauptwerk"))
                .Configure<Hauptwerk.MotuAvbConfiguration>(Configuration.GetSection("motu"))
                .AddSingleton<Hauptwerk.IHauptwerkExeHelper, Hauptwerk.HauptwerkExeHelper>()
                .AddSingleton<Hauptwerk.IAudioCardHelper, Hauptwerk.MotuAvbAudioCardHelper>()
                .AddSingleton<Archive.IMidiArchiveHandler, Archive.MidiArchiveHandler>()
                .AddSingleton<Midi.IMidiMessageSerializer, Midi.Impl.MessageSerializer>()
                .AddSingleton<Midi.IMidiInterface, Midi.Impl.MidiInterface>()
                .AddAlwaysOnSingleton<Hauptwerk.IHauptwerkMidiInterface, Hauptwerk.HauptwerkMidiInterface>()
                .AddSingleton<IPcService, PcService>()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseMvc()
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
