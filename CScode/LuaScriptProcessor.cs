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

namespace Lorei
{
    class LuaScriptProcessor : ScriptProcessor
    {
        /************ Constructors ************/
        public LuaScriptProcessor(LoreiLanguageProcesser lorei)
        {
            // Save pointer to Lorei
            m_owner = lorei;
            
            // Setup LUA functions
            SetupLuaFunctions();
            
            //Well this was much simpler than expected.
            this.LoadScripts();
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
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }

        }

        /************ Helper Methods ************/
        // Helper Methods For Lua Engine
        private void SetupLuaFunctions()
        {
            // Start up the lua engine
            m_luaEngine = new Lua();

            // Setup global functions
            m_luaEngine.RegisterFunction("SendMessage", m_owner, m_owner.GetType().GetMethod("DispatchMessageToWindow"));
            m_luaEngine.RegisterFunction("LaunchProgram", m_owner, m_owner.GetType().GetMethod("LaunchProgram"));
            m_luaEngine.RegisterFunction("RegisterProgramWithScript", this, this.GetType().GetMethod("RegisterProgramWithScript"));
            RegisterFunctionTemplate("ExitProgram");
            RegisterFunctionTemplate("ExitStubbornProgram");
            RegisterFunctionTemplate("RegisterLoreiName");
            RegisterFunctionTemplate("RegisterLoreiFunction");
            RegisterFunctionTemplate("RegisterLoreiProgramName");
            RegisterFunctionTemplate("RegisterLoreiProgramCommand");
            RegisterFunctionTemplate("SayMessage");
        }
        public void RegisterProgramWithScript(string p_ProgramName)
        {
            this.DoFile("Scripts/" + p_ProgramName + ".lua");
        }
        private void RegisterFunctionTemplate(string functionName)
        {
            m_luaEngine.RegisterFunction(functionName, m_owner, m_owner.GetType().GetMethod(functionName));
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

        /************ Data ************/
        // Lua Engine Data
        Lua m_luaEngine;
        LoreiLanguageProcesser m_owner;
    }
}
