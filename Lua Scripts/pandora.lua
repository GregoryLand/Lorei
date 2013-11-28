-- Setup Command Line
RegisterLoreiProgramName("pandora");

-- Setup Keywords
-- Got nothin right now

-- Functions
if( command == nil) then
	command = {}
end

command.FilePath = "C:\\Program Files (x86)\\Pandora\\Pandora.exe";


--ToLaunch
function command.ToLaunch()
	LaunchProgram(command.FilePath);
end;

function command.ToClose()
	ExitProgram(command.FilePath);
end;

function command.OnSpeechReceved( e )
end;