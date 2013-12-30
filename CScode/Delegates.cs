using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Lorei
{
    // Delegates for the Processor
    delegate void ParseSpeech(LoreiLanguageProcesser sender, SpeechRecognizedEventArgs data);
    delegate void ProcesserSwitchChanged( LoreiLanguageProcesser sender, Boolean newState);
}
