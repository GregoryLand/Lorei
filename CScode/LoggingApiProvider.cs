using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lorei
{
    public class LoggingApiProvider : ApiProvider
    {
        /************ Constructors ************/
        public LoggingApiProvider()
        {
            log = LogManager.GetLogger(typeof(LoggingApiProvider));
        }

        public void Info(String infoString)
        {
            log.Info(infoString);
        }

        public void Error(String errorString)
        {
            log.Error(errorString);
        }

        public void Debug(String debugString)
        {
            log.Debug(debugString);
        }

        private static ILog log;
    }
}
