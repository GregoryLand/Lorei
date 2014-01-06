using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Lorei
{
    // Delegates for the Processer
    public delegate void ParseSpeech(LoreiLanguageProvider sender, SpeechRecognizedEventArgs data);
    public delegate void ProcesserSwitchChanged( LoreiLanguageProvider sender, Boolean newState);

}
