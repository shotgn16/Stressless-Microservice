using NLog;
using System.Diagnostics;

namespace Stressless_Service.logging
{
    public class NLogTraceListener : TraceListener
    {
        private readonly Logger _logger;

        public NLogTraceListener()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override void WriteLine(string message)
        {
            _logger.Debug(message);
        }

        public override void Write(string message)
        {
            _logger.Debug(message);
        }
    }
}
