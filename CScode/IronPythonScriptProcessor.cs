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
        public IronPythonScriptProcessor(LoreiLanguageProcesser p_owner)
        {
            m_owner = p_owner;

            // Setup python
            test = m_pythonEngine.CreateScope();
            test.SetVariable("LoreiApi", m_owner);
            m_pythonEngine.ExecuteFile("Scripts/Test.py", test);

            //m_pythonEngine.Globals.SetVariable("LoreiApi", m_owner);
            //test = m_pythonEngine.CreateScope();
            //test = m_pythonEngine.ExecuteFile("Scripts/Test.py");//UseFile("Scripts/Test.py");
        }

        /************ Methods ************/
        // Script Processor Interface
        public void ParseSpeech(System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            //throw new NotImplementedException();
            test.Simple();
        }

        /************ Helper Methods ************/

        /************ Data ************/
        //Microsoft.Scripting.Hosting.ScriptRuntime m_pythonEngine = Python.CreateRuntime();
        Microsoft.Scripting.Hosting.ScriptEngine m_pythonEngine = Python.CreateEngine();
        public LoreiLanguageProcesser m_owner;
        dynamic test;
    }
}
