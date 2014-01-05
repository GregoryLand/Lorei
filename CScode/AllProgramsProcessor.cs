using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;

namespace Lorei
{
    //Pretending to be a ScriptProcessor to make things easier. It works. I hope.
    class AllProgramsProcessor : ScriptProcessor
    {
        public AllProgramsProcessor(LoreiLanguageProcessor p_owner, ApiDictionary apiDictionary)
        {
            m_owner = p_owner;
            m_ProcApi = (ProcessApiProvider)apiDictionary.getApiProvider("ProcessApi");

            //Get the list of programs. Returns a List with keyvalue pairs of just the shortcut name, Full path
            m_ListOfPrograms = GetAllShortcuts();

            //Register them.
            RegisterAllProgramsWithLorei(m_ListOfPrograms);
        }

        Dictionary<String, String> GetAllShortcuts()
        {
            Dictionary<String, String> theShortCuts = new Dictionary<String, String>();

            String[] startMenuShortCuts = Directory.GetFiles("C:\\ProgramData\\Microsoft\\Windows\\Start Menu", 
                "*.lnk", SearchOption.AllDirectories);
            String[] userDesktopShortCuts = Directory.GetFiles("C:\\Users\\" + Environment.UserName + "\\Desktop", 
                "*.lnk", SearchOption.AllDirectories);
            String[] publicDesktopShortCuts = Directory.GetFiles("C:\\Users\\Public\\Desktop", "*.lnk", SearchOption.AllDirectories);

            theShortCuts = CreateDictionaryFromArray(theShortCuts, startMenuShortCuts);
            theShortCuts = CreateDictionaryFromArray(theShortCuts, userDesktopShortCuts);
            theShortCuts = CreateDictionaryFromArray(theShortCuts, publicDesktopShortCuts);
            return theShortCuts;
        }
        
        //Merges the Arrays into the Dictionary. Also checks for multiples(Won't add multiples keys)
        Dictionary<String, String> CreateDictionaryFromArray(Dictionary<String, String> theDictionary, String[] theArray)
        {
            foreach (String arrayElement in theArray)
            {
                if (!theDictionary.ContainsKey(Path.GetFileNameWithoutExtension(arrayElement)))
                {
                    theDictionary.Add(Path.GetFileNameWithoutExtension(arrayElement), arrayElement);
                }
            }

            return theDictionary;
        }

        //Registering the Programs
        void RegisterAllProgramsWithLorei(Dictionary<String, String>keysToRegister)
        {
            foreach (KeyValuePair<String, String>registerProgram in keysToRegister)
            {
                m_owner.RegisterLoreiProgramName(registerProgram.Key);
            }
        }

        public void ParseSpeech(SpeechRecognizedEventArgs e)
        {
            //Nothing happens yet.
            Console.WriteLine(e.ToString());
            switch(e.Result.Words[1].Text) {
                case "Launch":
                    LaunchProgram(e.Result.Words[2].Text);
                    break;
                case "Close":
                    ExitProgram(e.Result.Words[2].Text);
                    break;
                default:
                    //Nothing
                    break;
            }
                
        }

        //Overriding default behavior to check for keys.
        private void LaunchProgram(string key)
        {
            if (m_ListOfPrograms.ContainsKey(key))
                m_ProcApi.LaunchProgram(m_ListOfPrograms[key]);
        }
        private void ExitProgram(string key)
        {
            if (m_ListOfPrograms.ContainsKey(key))
                m_ProcApi.ExitProgram(m_ListOfPrograms[key]);
        }

        //This is allow scripts to be able to grab a path based on the program name, rather than a hardcoded path.
        public String GetFilePath(String key)
        {
            String value = "";

            if (m_ListOfPrograms.ContainsKey(key))
            {
                value = m_ListOfPrograms[key];
            }

            return value;
        }

        ProcessApiProvider m_ProcApi;
        LoreiLanguageProcessor m_owner;
        Dictionary<String, String> m_ListOfPrograms;
    }
}
