/**************************************************************
 * Program: Lorei
 * Class:
 * Description:
 *  This class handles all of the logic required for lorei to
 *  operate.  The class is currently designed around a 
 *  Dictionary that is used to store and look up created
 *  processes based on a file path to the launched exe.
 *  This may or may not be the best solution but it works well.
 *     drawbacks: 1) Can only have one instance of a program 
 *                open at a time. But since i haven't created a 
 *                way to identify more then one program instance
 *                at a time this is a mute point.
 *  The class primarily focuses on the creation and handling of 
 *  the grammar classes used to control different programs.  The
 *  grammar classes are what allow the speech api to understand 
 *  English.
 *  This class also hosts the script engines that setup the
 *  the grammars required for a 
 *  specific program this provides a large amount of flexibility 
 *  and change to programs without the need to recompile lorei. 
 *  This class acts as a interface providing some basic methods
 *  that the scripting engines can expose in the script files. 
 **************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using log4net;

namespace Lorei
{
    public class LoreiLanguageProvider: ApiProvider
    {
        /************ Constructors ************/
        public LoreiLanguageProvider(TextToSpeechApiProvider p_textToSpeechApi)
        {
            m_textToSpeechApi = p_textToSpeechApi;

            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(LoreiLanguageProvider));

            // Setup Engine
            SetupSpeechRecognitionEngine();
        }

        /************ Destructor ************/
        /************ Methods ************/
        public void LoreiStartListening()
        {
            if (!m_Enabled)
            {
                LoadSpeechInformation();

                m_speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                if (StateChanged != null) StateChanged(this, true);
                m_Enabled = true;
                log.Info("Lorei has started listening.");
            }
        }
        public void LoreiStopListening()
        {
            if (m_Enabled)
            {
                m_speechRecognizer.RecognizeAsyncCancel();
                if (StateChanged != null) StateChanged(this, false);
                m_Enabled = false;
                log.Info("Lorei has stopped listening.");
            }
        }
        public void LoadScriptProcessor(ScriptProcessor p_scriptProcessor)
        {
            m_scriptProcessors.Add(p_scriptProcessor);
            log.Info(p_scriptProcessor + " has been added to the list of Script Processors.");
        }

        //Methods for Registration Api
        public void RegisterLoreiName(string p_NameForLorei)
        {
            // Do the work.... shame to need a function for this
            RegisterTemplate(p_NameForLorei, m_Keywords);
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
            log.Info("Registration is complete.");
        }

        /************ Api Provider Interface ************/
        public List<System.Reflection.MethodInfo> GetMethods()
        {
            List<System.Reflection.MethodInfo> methods = new List<System.Reflection.MethodInfo>();

            // Setup the list
            methods.Add(this.GetType().GetMethod("RegisterLoreiName"));
            methods.Add(this.GetType().GetMethod("RegisterLoreiFunction"));
            methods.Add(this.GetType().GetMethod("RegisterLoreiProgramName"));
            methods.Add(this.GetType().GetMethod("RegisterLoreiProgramCommand"));
            methods.Add(this.GetType().GetMethod("RegistrationDone"));

            return methods;
        }

        // Event Handlers
        private void m_speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Interaction Message
            m_textToSpeechApi.SayMessage("Ok!");
            
            // Parse Speech
            ParseSpeech(e);
            log.Info("Message: " + e.Result.Text);
        }
        private void m_speechRecognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            // If we knew any words
            if (e.Result.Words.Count > 0)
            {
                m_textToSpeechApi.SayMessage("What?");
            }
        }

        /************ Helper Methods ************/
        // Helper Methods For Speech Recognition Engine
        private void SetupSpeechRecognitionEngine()
        {
            // Setup Speech Engine
            m_speechRecognizer = new SpeechRecognitionEngine();

            if (m_speechRecognizer == null)
            {
                m_textToSpeechApi.SayMessage("Speech Recognizer Creation Failed is Null");
                log.Error("Speech Recognizer has failed to created.");
            }
            else
            {
                m_textToSpeechApi.SayMessage("Speech Recognizer Created");
                
                // Bind to default audio device
                m_speechRecognizer.SetInputToDefaultAudioDevice();

                // Setup Event Handlers
                SetupEventHandlers();
            }
        }
        private void LoadSpeechInformation()
        {
            // Data
            Choices keywords;
            Choices functions;
            Choices programs;
            //Choices actions;
            Choices programActions;

            // Setup Grammars
            m_FunctionExecution  = new GrammarBuilder();
            m_ProgramControl     = new GrammarBuilder();

            // Setup Keywords
            keywords  = new Choices(m_Keywords.ToArray());
            // Setup Function List
            functions = new Choices(m_Functions.ToArray());
            // Setup List of Programs;
            programs  = new Choices(m_Programs.ToArray()); 
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
            m_ProgramGrammar  = new Grammar(m_ProgramControl);
            m_FunctionGrammar.Name = "m_FunctionGrammar";
            m_ProgramGrammar.Name  = "m_ProgramGrammar";

            m_speechRecognizer.LoadGrammar(m_FunctionGrammar);
            m_speechRecognizer.LoadGrammar(m_ProgramGrammar);
        }
        private void SetupEventHandlers()
        {
            m_speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(m_speechRecognizer_SpeechRecognized);
            m_speechRecognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(m_speechRecognizer_SpeechRecognitionRejected);
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

        // Helper Methods For Parsing Speech and script Api accessible functions 
        private void ParseSpeech(SpeechRecognizedEventArgs e)
        {
            // Check if disabled by voice
            // HACK::::::HACK::::::HACK
            if (m_disabledByVoice == true)
            {
                if (e.Result.Words[1].Text != "Enable") return;
                if (e.Result.Words[2].Text != "Speech") return;
                m_disabledByVoice = false;
                return;
            }
            // ENDHACK:::::ENDHACK::::ENDHACK

            // Let the world know we parsed a command
            m_lastCommand = e.Result.Text;
            this.TextReceived(this, e);

            // Pass the buck
            // Let our scripting languages have the message.
            // TODO: Clean up this interface later
            foreach (ScriptProcessor x in m_scriptProcessors)
            {
                x.ParseSpeech(e);
            }
        }

        /************ Constants ************/

        /************ Events ************/
        public event ParseSpeech TextReceived;
        public event ProcesserSwitchChanged StateChanged;

        /************ Accessors ************/
        public bool Active 
        {
            set
            {
                if (value == true) LoreiStartListening();
                else LoreiStopListening();
            }
            get
            {
                return m_Enabled;
            }
        }
        public string LastCommand
        {
            get
            {
                return m_lastCommand;
            }
            
        }

        /************ Data ************/
        private List<String> m_Keywords       = new List<string>();
        private List<String> m_Functions      = new List<string>();
        private List<String> m_Programs       = new List<string>();
        private List<String> m_Aliases        = new List<string>();
        private List<String> m_ProgramActions = new List<string>();
        
        // Speech Components
        private SpeechRecognitionEngine m_speechRecognizer;
        private GrammarBuilder    m_FunctionExecution;
        private GrammarBuilder    m_ProgramControl;
        private Grammar m_FunctionGrammar;
        private Grammar m_ProgramGrammar;

        // Program Control Data
        private bool m_disabledByVoice = false;
        private bool m_Enabled = false;
        private string m_lastCommand;

        // Scripting Data
        private List<ScriptProcessor> m_scriptProcessors = new List<ScriptProcessor>();
        private TextToSpeechApiProvider m_textToSpeechApi;
        bool m_RegistrationComplete = false;
        
        //Logging Data
        private static ILog log;
    }
}
