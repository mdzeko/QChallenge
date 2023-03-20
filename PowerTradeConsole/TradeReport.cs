using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Services;

namespace PowerTradeConsole
{
    public class TradeReport
    {
        private Timer _timer;

        public string OutputFileDir { get; private set; }
        public int OutputIntervalInMinutes { get; private set; }

        public TradeReport()
        {

        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.GenerateReport();
        }

        private void GenerateReport()
        {
            string fileName = "PowerPosition_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
            StringBuilder sb = new StringBuilder();
            sb.Append("");


            using (StreamWriter outputFile = new StreamWriter(Path.Combine(this.OutputFileDir, fileName), false))
            {
                outputFile.Write(sb.ToString());
            }
        }

        public void Start()
        {
            this.OutputFileDir = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["OutputFileLocation"]);
            if (!Directory.Exists(this.OutputFileDir)) Directory.CreateDirectory(this.OutputFileDir);

            this.OutputIntervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["ReportIntervalInMinutes"]);
            int intervalInMiliseconds = this.OutputIntervalInMinutes * 60 * 1000;

            _timer = new Timer(intervalInMiliseconds) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
