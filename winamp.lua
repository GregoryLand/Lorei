--Setup Winamp
RegisterLoreiProgramName("Winamp")

--Setup Winamps keywords
RegisterLoreiProgramCommand("Play")
RegisterLoreiProgramCommand("Pause")
RegisterLoreiProgramCommand("Stop")
RegisterLoreiProgramCommand("Next")
RegisterLoreiProgramCommand("Previous")

------------ Object containing the Functions required by Lorei ------------
if( Winamp == nil) then
	Winamp = {}
end

Winamp.FilePath = "C:\\Program Files (x86)\\Winamp\\winamp.exe"
Winamp.WM_COMMAND = 0x0111
Winamp.PLAY     = 40045
Winamp.PAUSE    = 40046
Winamp.STOP     = 40047
Winamp.PREVIOUS = 40044
Winamp.NEXT     = 40048

--Functions
-- ToLaunch
function Winamp.ToLaunch()
	LaunchProgram(Winamp.FilePath)
end

function Winamp.ToClose()
	ExitProgram(Winamp.FilePath)
end



-- On Speech Receved
function Winamp.OnSpeechReceved( e )
	if e == "Play" then
		SendMessage( Winamp.FilePath, Winamp.WM_COMMAND, Winamp.PLAY    , 0)
	elseif e == "Pause" then
		SendMessage( Winamp.FilePath, Winamp.WM_COMMAND, Winamp.PAUSE   , 0)
	elseif e == "Stop" then
		SendMessage( Winamp.FilePath, Winamp.WM_COMMAND, Winamp.STOP    , 0)
	elseif e == "Previous" then
		SendMessage( Winamp.FilePath, Winamp.WM_COMMAND, Winamp.PREVIOUS, 0)
	elseif e == "Next" then
		SendMessage( Winamp.FilePath, Winamp.WM_COMMAND, Winamp.NEXT    , 0)
	end
end