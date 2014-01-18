using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.ApiProviders
{
    public class LoggingApiProvider : ApiProvider
    {
        /**
         * Base Constructor which creates a Log4Net logger object for Scripts to use. 
         */
        public LoggingApiProvider()
        {
            log = LogManager.GetLogger(typeof(LoggingApiProvider));
        }

        /**
         * Direct link to Log4Net's Info logger method.
         * 
         * @param infoString The information to pass to the logger
         */
        public void Info(String infoString)
        {
            log.Info(infoString);
        }

        /**
         * Direct link to Log4Net's Error logger method.
         * 
         * @param errorString The error message to pass to the logger
         */
        public void Error(String errorString)
        {
            log.Error(errorString);
        }

        /**
         * Direct link to Log4Net's Debug logger method.
         * 
         * @param debugString The debug message to pass to the logger
         */
        public void Debug(String debugString)
        {
            log.Debug(debugString);
        }

        /************ Api Provider Interface ************/
        public List<System.Reflection.MethodInfo> GetMethods()
        {
            List<System.Reflection.MethodInfo> methods = new List<System.Reflection.MethodInfo>();

            // Setup the list
            methods.Add( this.GetType().GetMethod("Info") );
            methods.Add( this.GetType().GetMethod("Error") );
            methods.Add( this.GetType().GetMethod("Debug") );

            return methods;
        }

        /************ Data ************/
        private static ILog log;


    }
}
