using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Lorei
{
    // Delegates for the Processer
    public delegate void ParseSpeech(LoreiLanguageProcessor sender, SpeechRecognizedEventArgs data);
    public delegate void ProcesserSwitchChanged( LoreiLanguageProcessor sender, Boolean newState);

}
