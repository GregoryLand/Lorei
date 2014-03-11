# Converted this script from AllProgramsProcessor.cs
# Original Author: Skyler Sommers
import clr
clr.AddReference('System.Windows.Forms')
import System.Windows.Forms
clr.AddReference('System.Speech')
import System.Speech.Recognition
import System.Collections.Generic
import System.IO
# Lorei Api Stuff
import LoreiApi
import ProcessApi
import TextToSpeechApi

##### Setup Functions #####
def CreateDictionaryFromArray( theDictionary, theArray ):
    for arrayElement in theArray:
        if System.IO.Path.GetFileNameWithoutExtension(arrayElement) not in theDictionary:
            theDictionary.Add( System.IO.Path.GetFileNameWithoutExtension(arrayElement), arrayElement )
    return theDictionary

def EliminateLuaControlledItems( theDictionary ):
    # Index List of Lua Files in directory 
    luaFiles = System.IO.Directory.GetFiles(  System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Scripts", "*.lua", System.IO.SearchOption.AllDirectories);
    
    # Remove entries from the list that match out items
    for luaFileToCheckFor in luaFiles:
        theDictionary.Remove( System.IO.Path.GetFileNameWithoutExtension(luaFileToCheckFor ) )
        #System.Windows.Forms.MessageBox.Show( System.IO.Path.GetFileNameWithoutExtension(luaFileToCheckFor) )

    return theDictionary

def GetAllShortcuts( ):
    # Shortcut list
    theShortCuts = System.Collections.Generic.Dictionary[str,str]()

    # Grab the shortcuts from common places
    startMenuShortCuts     = System.IO.Directory.GetFiles("C:\\ProgramData\\Microsoft\\Windows\\Start Menu", "*.lnk", System.IO.SearchOption.AllDirectories)
    userDesktopShortCuts   = System.IO.Directory.GetFiles("C:\\Users\\" + System.Environment.UserName + "\\Desktop", "*.lnk", System.IO.SearchOption.AllDirectories)
    publicDesktopShortCuts = System.IO.Directory.GetFiles("C:\\Users\\Public\\Desktop", "*.lnk", System.IO.SearchOption.AllDirectories)          
    
    # setup the dictionary
    theShortCuts = CreateDictionaryFromArray( theShortCuts, startMenuShortCuts )
    theShortCuts = CreateDictionaryFromArray( theShortCuts, userDesktopShortCuts )
    theShortCuts = CreateDictionaryFromArray( theShortCuts, publicDesktopShortCuts )

    # Remove any Lua script controlled programs from the list
    theShortCuts = EliminateLuaControlledItems( theShortCuts )

    return theShortCuts

##### Setup and Create List of Programs to pick from #####
ProgramsFound = System.Speech.Recognition.Choices()

#Get the list of programs. Returns a List with keyvalue pairs of just the shortcut name, Full path          
m_ListOfPrograms = GetAllShortcuts()

#Use the list to setup the Choices
for key in m_ListOfPrograms.Keys:
    ProgramsFound.Add(key)

##### Setup Grammar #####
# Setup Launch Phrase
LaunchPhrase = System.Speech.Recognition.GrammarBuilder()
LaunchPhrase.Append( System.Speech.Recognition.Choices( LoreiApi.GetLoreiNames().ToArray() ) )
LaunchPhrase.Append("Launch")
LaunchPhrase.Append( ProgramsFound )

# Setup Exit Phrase
ExitPhrase = System.Speech.Recognition.GrammarBuilder()
ExitPhrase.Append( System.Speech.Recognition.Choices( LoreiApi.GetLoreiNames().ToArray() ) )
ExitPhrase.Append("Close")
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
            #Do Launch Stuff
            #TextToSpeechApi.SayMessage("Launching with Python")
            if speechEvent.Result.Words[2].Text in m_ListOfPrograms:
                ProcessApi.LaunchProgram( m_ListOfPrograms[speechEvent.Result.Words[2].Text].ToString() )
        elif speechEvent.Result.Words[1].Text == "Close":
            #Do Close Stuff
            #TextToSpeechApi.SayMessage("Closeing with Python")
            if speechEvent.Result.Words[2].Text in m_ListOfPrograms:
                ProcessApi.ExitProgram( m_ListOfPrograms[speechEvent.Result.Words[2].Text].ToString() )
    return
