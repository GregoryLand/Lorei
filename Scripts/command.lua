-- Setup Command Line
RegisterLoreiProgramName("command")

-- Setup Keywords
-- Got nothin right now

-- Functions
if( command == nil) then
	command = {}
end

command.FilePath = "C:\\Windows\\system32\\cmd.exe"


--ToLaunch
function command.ToLaunch()
	LaunchProgram(command.FilePath)
end

function command.ToClose()
	ExitProgram(command.FilePath)
end

function command.OnSpeechReceved( e )
end