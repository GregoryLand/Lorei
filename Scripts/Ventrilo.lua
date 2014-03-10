-- Setup Command Line
RegisterLoreiProgramName("ventrilo")

-- Setup Keywords
-- Got nothin right now

-- Functions
if( ventrilo== nil) then
	ventrilo= {}
end

ventrilo.FilePath = "C:\\Program Files\\Ventrilo\\Ventrilo.exe"


--ToLaunch
function ventrilo.ToLaunch()
	LaunchProgram(ventrilo.FilePath)
end;

function ventrilo.ToClose()
	ExitProgram(ventrilo.FilePath)
end;

function ventrilo.OnSpeechReceved( e )
end;
