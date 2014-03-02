using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// Speech
using System.Speech.Recognition;
// Libs
using NLua;
using Microsoft.Win32;
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.Processors
{
    class LuaScriptProcessor : ScriptProcessor
    {
        /************ Constructors ************/
        public LuaScriptProcessor( IDictionary<string, ApiProvider> apiDictionary)
        {
            // Save pointer to Api Listing
            m_apiListing = apiDictionary;

            // Do something horrible to support old API used by Lua scripts
            foreach (String x in apiDictionary.Keys)
            {
                if (x == "LoreiApi")
                {
                    // Grab the LoreiApi and cast it so we have access to it so we can expose the old api
                    m_LoreiApi = (LoreiLanguageProvider)apiDictionary[x];
                    break;
                }
            }

            // Setup LUA functions
            SetupLuaFunctions();
            SetupOldLoreiApiMethods();

            //Well this was much simpler than expected.
            this.LoadScripts();

            // Setup the old grammars 
            LoadSpeechInformation();
        }

        /************ Methods ************/
        public void DoFile(string p_fileToDo)
        {
            this.m_luaEngine.DoFile(p_fileToDo);
        }
        public void ParseSpeech(SpeechRecognizedEventArgs e)
        {
            // This function calls the appropriate function in the Lua file so that 
            // programs can be controlled from the lua script and Lorei needs to know
            // nothing about the code that is ran there.
            // This creates a nice clean separation between dispatch and
            // operation. Lorei is the dispatcher and LUA is the actual thing doing
            // all of the work. Most of the time the scripts just call back to Lorei
            // functions to do all of the actual "Work" but it still prevents a lot of
            // boiler plate code.  This replaces a lot of stupid code. Look below to see
            // what i replaced.  Every program would have had to be hard coded into Lorei
            // and this system works much much better.
            try
            {
                switch (e.Result.Grammar.Name)
                {
                    case "m_FunctionGrammar":
                        string s = e.Result.Words[2].Text + ".To" + e.Result.Words[1].Text + "()";
                        m_luaEngine.DoString(s);
                        break;
                    case "m_ProgramGrammar":

                        // Rebuild the string from words to pass to the Lua engine.
                        string textToPass = "";
                        for (int x = 2; x < e.Result.Words.Count; x++)
                        {
                            if (x > 2) textToPass += " ";
                            textToPass += e.Result.Words[x].Text;
                        }

                        // If this throws make sure your scripts are correct.  capitalization must match on RegisterLoreiProgramName and the Lua object itself
                        m_luaEngine.GetFunction(e.Result.Words[1].Text + ".OnSpeechReceved").Call(textToPass);//e.Result.Words[2].Text);
                        break;
                    default:
                        break;
                }
            }  
            catch (NLua.Exceptions.LuaScriptException lse)
            {
                //Not sure why, but the all out exception wasn't catching this.
                //Need a better way to choose what's running the commands. This is thrown everything somethng from AllProgramsProcessor is said.
                //This is due to both using launch.
                Console.WriteLine(lse.StackTrace);
                //throw lse;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                //throw exception;
            }

        }

        /************ Helper Methods ************/
        // Helper Methods For Lua Engine
        private void SetupLuaFunctions()
        {
            // Start up the lua engine
            m_luaEngine = new Lua();

            // Run through each api and expose the methods provided
            // TODO: Setup each script api to have its own namespace
            // right now we just dump them all out in global space till i have
            // time to re write all of the scripts.
            foreach (ApiProvider x in m_apiListing.Values)
            {
                foreach (System.Reflection.MethodInfo method in x.GetMethods())
                {
                    RegisterFunctionTemplate(method.Name, x);
                }
            }
        }
        public void RegisterProgramWithScript(string p_ProgramName)
        {
            this.DoFile("Scripts/" + p_ProgramName + ".lua");
        }
        private void RegisterFunctionTemplate(string functionName, object owner)
        {
            m_luaEngine.RegisterFunction(functionName, owner, owner.GetType().GetMethod(functionName));
        }
        private void RegisterTemplate(string p_String, List<string> p_list)
        {
            // This function is a template to create other functions because
            // I'm lazy and copy paste is a bad idea so i make this...
            if (m_RegistrationComplete) return;

            // Check each element in list to see if new item exists already
            foreach (string x in p_list)
            {
                if (x == p_String) return;
            }

            p_list.Add(p_String);
        }
        private void LoadScripts()
        {
            String[] scripts = Directory.GetFiles("Scripts/");
            foreach (String script in scripts)
            {
                if (script.EndsWith(".lua"))
                {
                    this.DoFile(script);
                }
            }
        }
        
        // Setup the old grammars
        private void SetupOldLoreiApiMethods()
        {
            RegisterFunctionTemplate("RegisterLoreiName", this);
            RegisterFunctionTemplate("RegisterLoreiFunction", this);
            RegisterFunctionTemplate("RegisterLoreiProgramName", this);
            RegisterFunctionTemplate("RegisterLoreiProgramCommand", this);
            RegisterFunctionTemplate("RegistrationDone", this);
        }
        private void LoadSpeechInformation()
        {
            /*** Fake the old api from lorei ***/
            // Data
            Choices keywords;
            Choices functions;
            Choices programs;
            Choices programActions;

            // Grammar Stuff
            GrammarBuilder m_FunctionExecution;
            GrammarBuilder m_ProgramControl;

            // Setup Grammars
            m_FunctionExecution = new GrammarBuilder();
            m_ProgramControl = new GrammarBuilder();

            // Setup Keywords
            keywords = new Choices(m_Keywords.ToArray());
            // Setup Function List
            functions = new Choices(m_Functions.ToArray());
            // Setup List of Programs;
            programs = new Choices(m_Programs.ToArray());
            // Program Functions
            programActions = new Choices(m_ProgramActions.ToArray());

            // Setup Grammar
            m_FunctionExecution.Append(keywords);
            m_FunctionExecution.Append(functions);
            m_FunctionExecution.Append(programs);
            m_ProgramControl.Append(keywords);
            m_ProgramControl.Append(programs);
            m_ProgramControl.Append(programActions);

            // Setup Engine
            m_FunctionGrammar = new Grammar(m_FunctionExecution);
            m_ProgramGrammar = new Grammar(m_ProgramControl);
            m_FunctionGrammar.Name = "m_FunctionGrammar";
            m_ProgramGrammar.Name = "m_ProgramGrammar";

            m_LoreiApi.RegisterLoreiGrammar(m_FunctionGrammar);
            m_LoreiApi.RegisterLoreiGrammar(m_ProgramGrammar);
        }

        // Lua specific Api functions.  Holdover from old days to keep lua scripts running
        public void RegisterLoreiName(string p_NameForLorei)
        {
            // Do the work.... shame to need a function for this
            RegisterTemplate(p_NameForLorei, m_Keywords);

            // Add it to lorei as well for now
            // HACK::
            this.m_LoreiApi.AddLoreiName(p_NameForLorei);
        }
        public void RegisterLoreiFunction(string p_NameOfFunction)
        {
            RegisterTemplate(p_NameOfFunction, m_Functions);
        }
        public void RegisterLoreiProgramName(string p_NameOfProgram)
        {
            RegisterTemplate(p_NameOfProgram, m_Programs);
        }
        public void RegisterLoreiProgramCommand(string p_NameOfProgramCommand)
        {
            RegisterTemplate(p_NameOfProgramCommand, m_ProgramActions);
        }
        public void RegistrationDone()
        {
            // Check to see if i should be called
            if (m_RegistrationComplete) return;

            // Set flag so we disable all register functions
            m_RegistrationComplete = true;
            //log.Info("Registration is complete.");
        }

        /************ Data ************/
        // Lua Engine Data
        Lua m_luaEngine;
        IDictionary<string, ApiProvider> m_apiListing;

        /************ Fake the old lua script interface ************/
        // Ref to language api to fake old functions that use to exist here
        LoreiLanguageProvider m_LoreiApi;
        // Keep lists of keywords that we use to keep track of in Language Proc/Prov
        private List<String> m_Keywords = new List<string>();
        private List<String> m_Functions = new List<string>();
        private List<String> m_Programs = new List<string>();
        private List<String> m_Aliases = new List<string>();
        private List<String> m_ProgramActions = new List<string>();
        // Old Grammars we use to have in Language Proc
        private Grammar m_FunctionGrammar;
        private Grammar m_ProgramGrammar;
        // Let lua script proc know we are done with registration
        bool m_RegistrationComplete = false;
    }
}
