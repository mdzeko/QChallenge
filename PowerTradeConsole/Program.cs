using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Topshelf;

namespace PowerTradeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCodeEnum = HostFactory.Run(configurator =>
            {
                configurator.Service<TradeReport>(service =>
                {
                    service.ConstructUsing(tradeReporter => new TradeReport());
                    service.WhenStarted(tradeReporter => tradeReporter.Start());
                    service.WhenStopped(tradeReporter => tradeReporter.Stop());
                });
                
                configurator.RunAsLocalSystem();

                configurator.SetServiceName("TraderReportService");
            });

            int exitCode = (int)Convert.ChangeType(exitCodeEnum, exitCodeEnum.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
