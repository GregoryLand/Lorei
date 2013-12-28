using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Speech
using System.Speech.Recognition;

namespace Lorei
{
    interface ScriptProcessor
    {
        // Methods
        void ParseSpeech(SpeechRecognizedEventArgs e);
    }
}
