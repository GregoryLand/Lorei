using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Speech
using System.Speech.Recognition;

namespace Lorei.CScode.Interfaces
{
    public interface ScriptProcessor
    {
        // Methods
        void ParseSpeech(SpeechRecognizedEventArgs e);
    }
}
