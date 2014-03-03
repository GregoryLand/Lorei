using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.Processors
{
    /* This class handles the IronPython scripting.  Iron Python has access to the entire 
     * .net framework, standard python libs, as well as any api calls we decide to provide to it.
     * Currently it looks in the script folders for any python files and loads each 
     * python file into its own Script Scope.  The way this is setup means scripts 
     * will not interfere with each other.
     */
    class IronPythonScriptProcessor : ScriptProcessor
    {
        /************ Constructors ************/
        public IronPythonScriptProcessor(IDictionary<string, ApiProvider> apiDictionary)
        {
            //m_owner = p_owner;
            m_apiDictionary = apiDictionary;

            // Setup Assemblies
            // Could try catch here but without this mscorlib.dll we are screwed 6 ways till Sunday
            m_pythonEngine.LoadAssembly( System.Reflection.Assembly.Load("mscorlib.dll") );
            m_pythonEngine.LoadAssembly( typeof(System.Speech.Recognition.SpeechRecognizedEventArgs).Assembly );

            // Import required items
            //m_pythonEngine.ImportModule("System.Speech");

            // Setup python
            foreach (String x in apiDictionary.Keys)
            {
                m_pythonEngine.Globals.SetVariable(x, apiDictionary[x]);
            }

            this.ExecuteEachScript();
        }

        /************ Methods ************/
        // Script Processor Interface
        public void ParseSpeech(System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            // Run through all scripts and call the parse speech function on each one.
            foreach (dynamic x in m_listOfScopes)
            {
                if (x.ContainsVariable("ParseSpeech"))
                {
                    // TODO: At some point we will have to go threw and see what exceptions can happen here
                    // for now just catch them and rethrow.
                    try
                    {
                        x.ParseSpeech(e);
                    }
                    catch (Exception oCrap)
                    {
                        throw oCrap;
                    }
                }
            }
        }

        /************ Helper Methods ************/
        public void ExecuteEachScript()
        {
            String[] scripts = System.IO.Directory.GetFiles("Scripts/");

            foreach (String script in scripts)
            {
                if (script.EndsWith(".py"))
                {
                    m_listOfScopes.Add( m_pythonEngine.ExecuteFile(script) );
                }
            }
        }

        /************ Data ************/
        // Python Scripting Info
        Microsoft.Scripting.Hosting.ScriptRuntime m_pythonEngine = Python.CreateRuntime();
        List<dynamic> m_listOfScopes = new List<dynamic>();

        // Language Processor Info
        IDictionary<string, ApiProvider> m_apiDictionary;
    }
}
