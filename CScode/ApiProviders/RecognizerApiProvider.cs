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
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.ApiProviders
{
    public class RecognizerApiProvider: ApiProvider
    {
        /**
         * API Provider for the Speech recognizer. Includes Grammar creation
         * 
         * @param p_textToSpeechApi Allows Speech
         */
        public RecognizerApiProvider(TextToSpeechApiProvider p_textToSpeechApi)
        {
            m_textToSpeechApi = p_textToSpeechApi;

            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(RecognizerApiProvider));

            // Setup Engine
            SetupSpeechRecognitionEngine();
        }

        /**
         * Changes Lorei's state to Listening.
         */
        public void LoreiStartListening()
        {
            if (!m_Enabled)
            {
                LoadTrackedGrammars();

                m_speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                if (StateChanged != null) StateChanged(this, true);
                m_Enabled = true;
                log.Info("Lorei has started listening.");
            }
        }

        /**
         * Changes Lorei's state to not Listening.
         */
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

        /**
         * Loads in Script Processor, so it can pass Speech events to them.
         * 
         * @param p_scriptProcessor The current ScriptProcessor to load.
         */
        public void LoadScriptProcessor(ScriptProcessor p_scriptProcessor)
        {
            m_scriptProcessors.Add(p_scriptProcessor);
            log.Info(p_scriptProcessor + " has been added to the list of Script Processors.");
        }

        //Methods for Registration Api
        public void RegisterLoreiGrammar(System.Speech.Recognition.Grammar p_grammarToLoad)
        {
            m_speechRecognizer.LoadGrammar(p_grammarToLoad);
        }

        /************ Api Provider Interface ************/
        public List<System.Reflection.MethodInfo> GetMethods()
        {
            List<System.Reflection.MethodInfo> methods = new List<System.Reflection.MethodInfo>();

            // Setup the list
            methods.Add(this.GetType().GetMethod("RegisterLoreiGrammar"));

            return methods;
        }

        /**
         * Event triggered when Lorei understand what is being said
         */
        private void m_speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Interaction Message
            m_textToSpeechApi.SayMessage("Ok!");
            
            // Parse Speech
            ParseSpeech(e);
            log.Info("Message: " + e.Result.Text);
        }

        /**
         * Event triggered if no command is understood
         */
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
        private void LoadTrackedGrammars()
        {
            foreach(Grammar g in m_GrammarsLoaded)
            {
                m_speechRecognizer.LoadGrammar(g);
            }
        }
        private void SetupEventHandlers()
        {
            m_speechRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(m_speechRecognizer_SpeechRecognized);
            m_speechRecognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(m_speechRecognizer_SpeechRecognitionRejected);
        }

        // Helper Methods For Parsing Speech and script Api accessible functions 
        private void ParseSpeech(SpeechRecognizedEventArgs e)
        {
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
        private List<Grammar> m_GrammarsLoaded = new List<Grammar>();

        // Speech Components
        private SpeechRecognitionEngine m_speechRecognizer;

        // Program Control Data
        private bool m_disabledByVoice = false;
        private bool m_Enabled = false;
        private string m_lastCommand;

        // Scripting Data
        private List<ScriptProcessor> m_scriptProcessors = new List<ScriptProcessor>();
        private TextToSpeechApiProvider m_textToSpeechApi;

        
        //Logging Data
        private static ILog log;
    }
}
