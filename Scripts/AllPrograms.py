import clr
clr.AddReference('System.Windows.Forms')
import System.Windows.Forms
clr.AddReference('System.Speech')
import System.Speech.Recognition
import LoreiApi
import TextToSpeechApi

TextToSpeechApi.SayMessage("Entering All Programs Script")

##### Setup and Create List of Programs to pick from #####
ProgramsFound = System.Speech.Recognition.Choices()
ProgramsFound.Add("onomatopoeia")



##### Setup Grammar #####
# Setup Launch Phrase
LaunchPhrase = System.Speech.Recognition.GrammarBuilder()
LaunchPhrase.Append( System.Speech.Recognition.Choices("System", "Endora") )
LaunchPhrase.Append("Launch")
LaunchPhrase.Append( ProgramsFound )
# Setup Exit Phrase
ExitPhrase = System.Speech.Recognition.GrammarBuilder()
ExitPhrase.Append( System.Speech.Recognition.Choices("System", "Endora") )
ExitPhrase.Append("Exit")
ExitPhrase.Append( ProgramsFound )

# Setup Top Level Choices
TopLevelChoices = System.Speech.Recognition.Choices()
TopLevelChoices.Add(LaunchPhrase)
TopLevelChoices.Add(ExitPhrase)

# Register Grammar
TopLevelGrammar = System.Speech.Recognition.Grammar( TopLevelChoices.ToGrammarBuilder() )
TopLevelGrammar.Name = "AllProgramsPythonScriptGrammar"
LoreiApi.RegisterLoreiGrammar( TopLevelGrammar )

##### Parse Speech #####
def ParseSpeech( speechEvent ):
    if speechEvent.Result.Grammar.Name == "AllProgramsPythonScriptGrammar":
        if speechEvent.Result.Words[1].Text == "Launch":
            #DO Launch Stuff
            TextToSpeechApi.SayMessage("Launch")
        elif speechEvent.Result.Words[1].Text == "Close":
            #Do Close Stuff
            TextToSpeechApi.SayMessage("Close")
        else:
            return
    return
