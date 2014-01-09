using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// Speech
using System.Speech.Synthesis;
using Lorei.CScode.ApiProviders;
using Lorei.CScode.Processors;
using Lorei.CScode.Interfaces;
using Lorei.CScode;

namespace Lorei.Forms
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();

            // Setup Api Stuff
            LoggingApiProvider loggingApiProvider           = new LoggingApiProvider();
            TextToSpeechApiProvider textToSpeechApiProvider = new TextToSpeechApiProvider();
            // Setup Api Stuff that needs Text to speech support
            ProcessApiProvider processApiProvider = new ProcessApiProvider(textToSpeechApiProvider);
            RecognizerApiProvider p_Brain         = new RecognizerApiProvider(textToSpeechApiProvider);

            // Setup Language Provider
            m_Brain = p_Brain; // HAHAHAHA science.....
            SetupLanguageProvider();

            // Setup the Api Listing
            IDictionary<string, ApiProvider> apiListing = new ApiDictionary<string, ApiProvider>();
            apiListing.Add("TextToSpeechApi", textToSpeechApiProvider);
            apiListing.Add("ProcessApi", processApiProvider);
            apiListing.Add("LoggingApi",loggingApiProvider);
            apiListing.Add("LoreiApi", m_Brain);
            
            // Setup Scripting Languages
            m_Brain.LoadScriptProcessor(new LuaScriptProcessor(apiListing));
            m_Brain.LoadScriptProcessor(new IronPythonScriptProcessor(apiListing));
            //m_Brain.LoadScriptProcessor(new AllProgramsProcessor(m_Brain, apiListing));  // All programs processor is still all dirty and smelly. need to work on that.
        }

        void m_Brain_TextReceived(RecognizerApiProvider sender, System.Speech.Recognition.SpeechRecognizedEventArgs data)
        {
            this.lastCommandLabel.Text = m_Brain.LastCommand;
        }

        // Events
        private void m_Brain_StateChanged(RecognizerApiProvider sender, bool newState)
        {
            if (newState == true)
            {
                stateText.Text = "Enabled";
                return;
            }
            else
            {
                stateText.Text = "Disabled";
                return;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            m_Brain.Active = !m_Brain.Active;
        }

        // Helper Methods
        void SetupLanguageProvider()
        {
            // Set the Status Label to match current state
            if (m_Brain.Active)
            {
                stateText.Text = "Enabled";
            }
            else
            {
                stateText.Text = "Disabled";
            }

            
            // Setup Event Handlers
            m_Brain.StateChanged += new ProcesserSwitchChanged(m_Brain_StateChanged);
            m_Brain.TextReceived += new ParseSpeech(m_Brain_TextReceived);

            return;
        }
        
        // Data
        RecognizerApiProvider m_Brain;
     }
}