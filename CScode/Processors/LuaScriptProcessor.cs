using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
// Speech
using System.Speech.Recognition;
// Libs
using NLua;
using Microsoft.Win32;
using Lorei.CScode.Interfaces;
using Lorei.CScode.ApiProviders;

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
                    m_LoreiApi = (RecognizerApiProvider)apiDictionary[x];
                    break;
                }
            }

            // Setup LUA functions
            SetupLuaFunctions();
            SetupOldLoreiApiMethods();

            //Well this was much simpler than expected.
            this.LoadScripts();

            // Setup the old grammars 
            CreateAndLoadGrammars();
        }

        /************ Methods ************/
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
                /*** Check for keywords owned by functions ***/
                foreach (string functionName in m_dataFromSetupLua.m_Functions)
                {
                    if (e.Result.Words[1].Text == functionName)
                    {
                        string s = e.Result.Words[2].Text + ".To" + e.Result.Words[1].Text + "()";
                        m_luaEngine.DoString(s);
                        return;
                    }
                }
                // TODO: This is rather messy.  I would love to not have to use a n^2 search.  Be clever if things become a problem later on
                // Later on i can sort this info into an array 
                foreach (ScriptData x in m_ScriptInfo.Values)
                {
                    foreach (var y in x.m_Functions)
                    {
                        if (e.Result.Words[1].Text == y)
	                    {
                            string s = e.Result.Words[2].Text + ".To" + e.Result.Words[1].Text + "()";
                            m_luaEngine.DoString(s);
                            return;
	                    }
                    }
                }

                /*** Check for keywords owned by programs ***/
                foreach (ScriptData x in m_ScriptInfo.Values)
                {
                    foreach (var y in x.m_Programs)
                    {
                        if (e.Result.Words[1].Text == y)
                        {
                            // Rebuild the string from words to pass to the Lua engine.
                            string textToPass = "";
                            for (int z = 2; z < e.Result.Words.Count; z++)
                            {
                                if (z > 2) textToPass += " ";
                                textToPass += e.Result.Words[z].Text;
                            }

                            // If this throws make sure your scripts are correct.  capitalization must match on RegisterLoreiProgramName and the Lua object itself
                            m_luaEngine.GetFunction(e.Result.Words[1].Text + ".OnSpeechReceved").Call(textToPass);//e.Result.Words[2].Text);
                        }
                    }
                }
            }  
            catch (NLua.Exceptions.LuaScriptException lse)
            {
                //Not sure why, but the all out exception wasn't catching this.
                //Need a better way to choose what's running the commands. This is thrown everything somethng from AllProgramsProcessor is said.
                //This is due to both using launch.
                Console.WriteLine(lse.StackTrace);
                throw lse;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                throw exception;
            }
        }
        public void LoadScriptWithIsolatedScope(string p_fileToLLoad)
        {
            // Wipe the stack for the script to reconfigure
            this.m_dataForScriptsToSetup = new ScriptData();
            
            // Do the Script
            this.m_luaEngine.DoFile(p_fileToLLoad);

            // Setup long term storage for information from scripts
            this.m_ScriptInfo.Add(p_fileToLLoad, m_dataForScriptsToSetup);
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
            this.LoadScriptWithIsolatedScope("Scripts/" + p_ProgramName + ".lua");
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
                if ( script.Equals("Scripts/setup.lua") )
                {
                    // Load script as usual
                    this.LoadScriptWithIsolatedScope(script);

                    // Stash the info from the setup script
                    m_dataFromSetupLua = m_dataForScriptsToSetup;

                    // Remove the script from the dictionary so we don't try and process it 
                    m_ScriptInfo.Remove(script);

                    // Keep on looping
                    continue;
                }
                else
                {
                    if (script.EndsWith(".lua"))
                    {
                        this.LoadScriptWithIsolatedScope(script);
                    }
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
        private void CreateAndLoadGrammars()
        {
            List<GrammarBuilder> grammarBuffer = new List<GrammarBuilder>();
            Choices combinedGrammars = new Choices();

            // Run all data loaded
            foreach (ScriptData scriptItem in m_ScriptInfo.Values)
            {
                GrammarBuilder speechInfo = LoadSpeechInformation(scriptItem);
                if (speechInfo.ToString() != null)
                {
                    grammarBuffer.Add(speechInfo);
                }
            }

            // Setup the final choices
            combinedGrammars.Add(grammarBuffer.ToArray());

            // Setup the grammar
            if (grammarBuffer.Count > 0)
            {
                m_LoreiApi.RegisterLoreiGrammar(new Grammar(combinedGrammars.ToGrammarBuilder()));
            }
        }
        private GrammarBuilder LoadSpeechInformation(ScriptData p_scriptData)
        {
            /*** Fake the old api from lorei 
             * Run through the keywords and build the grammers for each of the functions
             * I also keep track of amout of entries added to each list.  There doesn't 
             * seem to be a way to keep to see if entires exist in the speech data structures
             * without building a parser(way to much work)
             ***/
            // Data
            Choices keywords; int countKeywords = 0;
            Choices functions; int countFunctions = 0;
            Choices programs; int countPrograms = 0;
            Choices programActions; int countProgramActions = 0;
            Choices finalChoices = new Choices(); int countFinalChoices = 0;

            // Grammar Stuff
            GrammarBuilder m_FunctionExecution = new GrammarBuilder();
            GrammarBuilder m_ProgramControl = new GrammarBuilder();
            GrammarBuilder m_GrammarToReturn = new GrammarBuilder();

            // Collect and setup all key words
            // Setup Keywords
            keywords = new Choices(p_scriptData.m_Keywords.ToArray());
            countKeywords += p_scriptData.m_Keywords.Count;
            keywords.Add(m_LoreiApi.GetLoreiNames().ToArray());
            countKeywords += m_LoreiApi.GetLoreiNames().Count;

            // Setup Function List
            functions = new Choices(p_scriptData.m_Functions.ToArray());
            countFunctions += p_scriptData.m_Functions.Count;
            functions.Add(m_dataFromSetupLua.m_Functions.ToArray());
            countFunctions += m_dataFromSetupLua.m_Functions.Count;
            foreach (var x in m_ScriptInfo.Values)
            {
                functions.Add(x.m_Functions.ToArray());
                countFunctions++;
            }
            
            // Setup List of Programs;
            programs = new Choices(p_scriptData.m_Programs.ToArray());
            countPrograms += p_scriptData.m_Programs.Count;

            // Program Functions
            programActions = new Choices(p_scriptData.m_ProgramActions.ToArray());
            countProgramActions += p_scriptData.m_ProgramActions.Count;

            // Setup Grammar for lorei functions
            if ( countKeywords > 0 && countFunctions > 0 && countPrograms > 0 )
            {
                m_FunctionExecution.Append(keywords);
                m_FunctionExecution.Append(functions);
                m_FunctionExecution.Append(programs);

                // Setup function grammars
                finalChoices.Add(m_FunctionExecution);

                // Keep track of number of final choices
                countFinalChoices++;
            }

            // Setup Grammar for program controls
            if (countKeywords > 0 && countPrograms > 0 && countProgramActions > 0)
            {
                m_ProgramControl.Append(keywords);
                m_ProgramControl.Append(programs);
                m_ProgramControl.Append(programActions);

                // Setup function grammars
                finalChoices.Add(m_ProgramControl);

                // Keep track of number of final choices
                countFinalChoices++;
            }

            if ( countFinalChoices > 0)
            {
                return finalChoices.ToGrammarBuilder();
                
            }
            else
            {
                return null;
            }
            // Setup Engine
            //m_FunctionGrammar = new Grammar(m_FunctionExecution);
            //m_ProgramGrammar = new Grammar(m_ProgramControl);
            //m_FunctionGrammar.Name = "m_FunctionGrammar";
            //m_ProgramGrammar.Name = "m_ProgramGrammar";
            
            //m_LoreiApi.RegisterLoreiGrammar(m_FunctionGrammar);
            //m_LoreiApi.RegisterLoreiGrammar(m_ProgramGrammar);
        }

        // Lua specific Api functions.  Holdover from old days to keep lua scripts running
        public void RegisterLoreiName(string p_NameForLorei)
        {
            // Do the work.... shame to need a function for this
            RegisterTemplate(p_NameForLorei, m_dataForScriptsToSetup.m_Keywords);

            // Add it to lorei as well for now
            // HACK::
            this.m_LoreiApi.AddLoreiName(p_NameForLorei);
        }
        public void RegisterLoreiFunction(string p_NameOfFunction)
        {
            RegisterTemplate(p_NameOfFunction, m_dataForScriptsToSetup.m_Functions);
        }
        public void RegisterLoreiProgramName(string p_NameOfProgram)
        {
            RegisterTemplate(p_NameOfProgram, m_dataForScriptsToSetup.m_Programs);
        }
        public void RegisterLoreiProgramCommand(string p_NameOfProgramCommand)
        {
            RegisterTemplate(p_NameOfProgramCommand, m_dataForScriptsToSetup.m_ProgramActions);
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
        RecognizerApiProvider m_LoreiApi;

        // Keep lists of keywords that we use to keep track of in Language Proc/Prov        
        // Data for each script
        private Dictionary<string, ScriptData> m_ScriptInfo = new Dictionary<string,ScriptData>();
        private volatile ScriptData m_dataForScriptsToSetup = new ScriptData();
        private volatile ScriptData m_dataFromSetupLua      = new ScriptData();
        
        // Let lua script proc know we are done with registration
        bool m_RegistrationComplete = false;

        /************ Struct for data storage ************/
        private class ScriptData
        {
            // Keep lists of keywords that we use to keep track of in Language Proc/Prov
            public List<String> m_Keywords = new List<string>();
            public List<String> m_Functions = new List<string>();
            public List<String> m_Programs = new List<string>();
            public List<String> m_Aliases = new List<string>();
            public List<String> m_ProgramActions = new List<string>();
        }
    }
}
