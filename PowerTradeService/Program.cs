using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Services;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Logging;

namespace PowerTradeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Power Trade reporter";
            });

            LoggerProviderOptions.RegisterProviderOptions<
            EventLogSettings, EventLogLoggerProvider>(builder.Services);

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddSingleton<IPowerService, PowerService>();

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            IHost host = builder.Build();
            host.Run();
        }
            
    }
}
