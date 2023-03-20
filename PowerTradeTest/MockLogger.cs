using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerTradeTest
{
    public class MockLogger<T> : ILogger<T>
    {
        private readonly string category = string.Empty;
        public MockLogger()
        {

        }
        public MockLogger(string categoryName)
        {
            this.category = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!string.IsNullOrEmpty(this.category)) System.Diagnostics.Debug.WriteLine($"{this.category}-> {formatter(state, exception)}");
            else System.Diagnostics.Debug.WriteLine(formatter(state, exception));
        }
    }
}
