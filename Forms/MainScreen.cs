using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// Speech
using System.Speech.Synthesis;


namespace Lorei
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();

            // Setup speech
            m_myBrain = new LoreiLanguageProcesser();

            // Set the Status Label to match current state
            if (m_myBrain.Active)
            {
                stateText.Text = "Enabled";
            }
            else
            {
                stateText.Text = "Disabled";
            }

            // Setup Event Handlers
            m_myBrain.StateChanged += new ProcesserSwitchChanged(m_myBrain_StateChanged);
            m_myBrain.TextReceived += new ParseSpeech(m_myBrain_TextReceived);

            // Setup Scripting Languages
            m_myBrain.LoadScriptProcessor(new LuaScriptProcessor(m_myBrain));
            //m_myBrain.LoadScriptProcessor(new IronPythonScriptProcessor(m_myBrain));
            m_myBrain.LoadScriptProcessor(new AllProgramsProcessor(m_myBrain));
        }

        void m_myBrain_TextReceived(LoreiLanguageProcesser sender, System.Speech.Recognition.SpeechRecognizedEventArgs data)
        {
            this.lastCommandLabel.Text = m_myBrain.LastCommand;
        }

        // Events
        private void m_myBrain_StateChanged(LoreiLanguageProcesser sender, bool newState)
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
            m_myBrain.Active = !m_myBrain.Active;
        }

        // Data
        LoreiLanguageProcesser m_myBrain;
     }
}