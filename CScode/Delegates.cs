using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Lorei.CScode.ApiProviders;

namespace Lorei.CScode
{
    // Delegates for the Processer
    public delegate void ParseSpeech(RecognizerApiProvider sender, SpeechRecognizedEventArgs data);
    public delegate void ProcesserSwitchChanged( RecognizerApiProvider sender, Boolean newState);

}
