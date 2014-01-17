using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using Lorei.CScode.Interfaces;

namespace Lorei.CScode.ApiProviders
{
    public class TextToSpeechApiProvider : ApiProvider
    {
        /************ Constructors ************/
        public TextToSpeechApiProvider()
        {
            // Setup Variables
            SetupSpeechSynthesizer();
        }

        /************ Methods ************/
        // Api accessible General Script Methods
        public void SayMessage(string p_Message)
        {
            // This makes the speech engine for the program say things
            // this function is made accessible to lua so scripts can say stuff
            m_speechSynthesizer.SpeakAsync(p_Message);
        }

        /************ Api Provider Interface ************/
        public List<System.Reflection.MethodInfo> GetMethods()
        {
            List<System.Reflection.MethodInfo> methods = new List<System.Reflection.MethodInfo>();

            // Setup the list
            methods.Add(this.GetType().GetMethod("SayMessage"));

            return methods;
        }


        /************ Helper Methods ************/
        // Helper Methods For Speech Synthesis Engine
        private void SetupSpeechSynthesizer()
        {
            // Start speech engine
            m_speechSynthesizer = new SpeechSynthesizer();

            // Do cute things with voice here
        }



        // Speech Components
        private SpeechSynthesizer m_speechSynthesizer;
    }
}
