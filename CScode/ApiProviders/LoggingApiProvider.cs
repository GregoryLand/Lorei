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
        /************ Constructors ************/
        public LoggingApiProvider()
        {
            log = LogManager.GetLogger(typeof(LoggingApiProvider));
        }

        /************ Methods ************/
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
