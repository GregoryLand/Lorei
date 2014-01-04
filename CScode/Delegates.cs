using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Lorei
{
    // Delegates for the Processer
    public delegate void ParseSpeech(LoreiLanguageProcesser sender, SpeechRecognizedEventArgs data);
    public delegate void ProcesserSwitchChanged( LoreiLanguageProcesser sender, Boolean newState);
}
