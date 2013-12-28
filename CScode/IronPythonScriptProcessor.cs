using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
namespace Lorei
{
    class IronPythonScriptProcessor : ScriptProcessor
    {
        /************ Constructors ************/
        public IronPythonScriptProcessor()
        {
            // Setup python
            m_pythonEngine = Python.CreateRuntime();
            dynamic test = m_pythonEngine.UseFile("Scripts/Test.py");
            test.Simple();
            
        }

        /************ Methods ************/
        // Script Processor Interface
        public void ParseSpeech(System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /************ Helper Methods ************/

        /************ Data ************/
        Microsoft.Scripting.Hosting.ScriptRuntime m_pythonEngine;
    }
}
