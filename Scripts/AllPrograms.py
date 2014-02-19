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
ProgramsFound.Add("Test")

##### Setup Grammar #####
# Setup Launch Phrase
LaunchPhrase = System.Speech.Recognition.GrammarBuilder()
LaunchPhrase.Append( System.Speech.Recognition.Choices("System", "Endora") )
LaunchPhrase.Append("Launch")
LaunchPhrase.Append( ProgramsFound )
# Setup Exit Phrase
ExitPhrase = System.Speech.Recognition.GrammarBuilder("Close")

# Setup Top Level Choices
TopLevelChoices = System.Speech.Recognition.Choices()
TopLevelChoices.Add(LaunchPhrase)
TopLevelChoices.Add(ExitPhrase)

# Register Grammar
LoreiApi.RegisterLoreiGrammar( System.Speech.Recognition.Grammar( TopLevelChoices.ToGrammarBuilder() ) )

###### Parse Speech ######
def ParseSpeech( speechEvent ):
    TextToSpeechApi.SayMessage("Event Happened")