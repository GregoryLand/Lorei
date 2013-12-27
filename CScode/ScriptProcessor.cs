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
        void DoFile(string p_fileToDo);
        void ParseSpeech(SpeechRecognizedEventArgs e);
    }
}
