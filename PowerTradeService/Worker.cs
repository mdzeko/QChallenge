using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;

namespace PowerTradeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPowerService _powerService;
        private readonly string _outputDir;
        private readonly int _reportInterval;


        public Worker(ILogger<Worker> logger, IPowerService powerService, IConfiguration appConfig)
        {
            this._powerService = powerService;
            this._outputDir = appConfig.GetValue<string>("ReportConfig:OutputFileDir");
            this._reportInterval = appConfig.GetValue<int>("ReportConfig:OutputIntervalInMinutes");
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            if (!Directory.Exists(this._outputDir)) Directory.CreateDirectory(this._outputDir);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                string fileName = "PowerPosition_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Local Time;Volume");
                IEnumerable<PowerTrade> tradesCollection = this._powerService.GetTrades(DateTime.Now);
                sb.Append(this.GetAggregateReport(tradesCollection));
                string dir = Environment.ExpandEnvironmentVariables(this._outputDir);
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(dir, fileName), false))
                {
                    streamWriter.Write(sb.ToString());
                }
                await Task.Delay(TimeSpan.FromMinutes(_reportInterval), stoppingToken);
            }
        }

        public string GetAggregateReport(IEnumerable<PowerTrade> tradesCollection)
        {
            var amountsByIndex = tradesCollection.SelectMany(trade => trade.Periods.Select((amount, index) => new { Amount = amount.Volume, Index = amount.Period }))
            .GroupBy(item => item.Index)
            .Select(group => new { Index = group.Key, Sum = group.Sum(item => item.Amount) });
            StringBuilder sb = new StringBuilder();
            foreach (var item in amountsByIndex)
            {
                sb.AppendLine($"{IndexLabel[item.Index]};{item.Sum}");
            }
            return sb.ToString();
        }

        private readonly Dictionary<int, string> IndexLabel = new Dictionary<int, string>
        {
            { 1, "23:00"},
            { 2, "00:00"},
            { 3, "01:00"},
            { 4, "02:00"},
            { 5, "03:00"},
            { 6, "04:00"},
            { 7, "05:00"},
            { 8, "06:00"},
            { 9, "07:00"},
            { 10, "08:00"},
            { 11, "09:00"},
            { 12, "10:00"},
            { 13, "11:00"},
            { 14, "12:00"},
            { 15, "13:00"},
            { 16, "14:00"},
            { 17, "15:00"},
            { 18, "16:00"},
            { 19, "17:00"},
            { 20, "18:00"},
            { 21, "19:00"},
            { 22, "20:00"},
            { 23, "21:00"},
            { 24, "22:00"},
        };
    }
}
