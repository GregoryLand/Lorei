--Setup Winamp
RegisterLoreiProgramName("winamp")

--Setup Winamps keywords
RegisterLoreiProgramCommand("Play")
RegisterLoreiProgramCommand("Pause")
RegisterLoreiProgramCommand("Stop")
RegisterLoreiProgramCommand("Next")
RegisterLoreiProgramCommand("Previous")

------------ Object containing the Functions required by Lorei ------------
if( winamp == nil) then
	winamp = {}
end

winamp.FilePath = "C:\\Program Files (x86)\\Winamp\\winamp.exe"
winamp.WM_COMMAND = 0x0111
winamp.PLAY     = 40045
winamp.PAUSE    = 40046
winamp.STOP     = 40047
winamp.PREVIOUS = 40044
winamp.NEXT     = 40048

--Functions
-- ToLaunch
function winamp.ToLaunch()
	LaunchProgram(winamp.FilePath)
end

function winamp.ToClose()
	ExitProgram(winamp.FilePath)
end



-- On Speech Receved
function winamp.OnSpeechReceved( e )
	if e == "Play" then
		SendMessage( winamp.FilePath, winamp.WM_COMMAND, winamp.PLAY    , 0)
	elseif e == "Pause" then
		SendMessage( winamp.FilePath, winamp.WM_COMMAND, winamp.PAUSE   , 0)
	elseif e == "Stop" then
		SendMessage( winamp.FilePath, winamp.WM_COMMAND, winamp.STOP    , 0)
	elseif e == "Previous" then
		SendMessage( winamp.FilePath, winamp.WM_COMMAND, winamp.PREVIOUS, 0)
	elseif e == "Next" then
		SendMessage( winamp.FilePath, winamp.WM_COMMAND, winamp.NEXT    , 0)
	end
end