-- Setup Command Line
RegisterLoreiProgramName("Visual Studio")

-- Setup Keywords
RegisterLoreiProgramCommand("Build")
RegisterLoreiProgramCommand("Debug")
RegisterLoreiProgramCommand("Start")

-- Functions
if( Visual == nil) then
	Visual = {}
end

Visual.FilePath = "C:\\Program Files (x86)\\Microsoft Visual Studio 10.0\\Common7\\IDE\\devenv.exe"
-- F6 for build command
Visual.BuildKey = 0x75
-- F5 for Debuging
Visual.LaunchWithDebugging = 0x74
-- Ctrl F5 for start without debugging
Visual.CtrlKey = 0x11
Visual.LaunchWithoutDebugging = 0x74
Visual.WM_KEYUP = 0x101
Visual.WM_KEYDOWN = 0x100

--ToLaunch
function Visual.ToLaunch()
	LaunchProgram(Visual.FilePath)
end

function Visual.ToClose()
	ExitProgram(Visual.FilePath)
end

function Visual.OnSpeechReceved( e )
	if e == "Studio Build" then
		SendMessage( Visual.FilePath, Visual.WM_KEYDOWN, Visual.BuildKey, 0 )
		SendMessage( Visual.FilePath, Visual.WM_KEYUP, Visual.BuildKey, 0 )
	elseif e == "Studio Debug" then
		SendMessage( Visual.FilePath, Visual.WM_KEYDOWN, Visual.LaunchWithDebugging, 0 )
		SendMessage( Visual.FilePath, Visual.WM_KEYUP, Visual.LaunchWithDebugging, 0 )
	elseif e == "Studio Start" then
		SendMessage( Visual.FilePath, Visual.WM_KEYDOWN, Visual.CtrlKey, 0 )
		SendMessage( Visual.FilePath, Visual.WM_KEYDOWN, Visual.LaunchWithoutDebugging, 0 )
		SendMessage( Visual.FilePath, Visual.WM_KEYUP, Visual.LaunchWithoutDebugging, 0 )
		SendMessage( Visual.FilePath, Visual.WM_KEYUP, Visual.CtrlKey, 0 )
	end
end