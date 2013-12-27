using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// Speech
using System.Speech.Synthesis;
// System Hook
using Kennedy.ManagedHooks;

namespace Lorei
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {
            InitializeComponent();

            // Setup speech
            m_myBrain = new LoreiLanguageProcesser();
            m_myBrain.StateChanged += new ProcesserSwitchChanged(m_myBrain_StateChanged);
            m_myBrain.TextReceived += new ParseSpeech(m_myBrain_TextReceived);

            // Setup keyboard hook 
            m_keyHook = new KeyboardHook();
            m_keyHook.KeyboardEvent += new KeyboardHook.KeyboardEventHandler(m_keyHook_KeyboardEvent);
        
            // Make sure lorei is waiting for a keypress to listen
            m_myBrain.Active = false;
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
        void m_keyHook_KeyboardEvent(KeyboardEvents kEvent, Keys key)
        {
            // Check if shift key is pushed
            if (key != Keys.LShiftKey) return;

            // Check if key was pushed down
            if (kEvent == KeyboardEvents.KeyDown)
            {
                m_myBrain.Active = true;
                return;
            }

            // Check if key was released and if it was stop listening
            if (kEvent == KeyboardEvents.KeyUp)
            {
                m_myBrain.Active = false;
                return;
            }
        }

        // Data
        LoreiLanguageProcesser m_myBrain;
        KeyboardHook m_keyHook;
     }
}