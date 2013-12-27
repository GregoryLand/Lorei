--Setup Chrome
RegisterLoreiProgramName("Chrome")

--Setup Chrome keywords
RegisterLoreiProgramCommand("New Tab")

------------ Object containing the Functions required by Lorei ------------
if( Chrome == nil) then
	Chrome = {}
end

Chrome.FilePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
Chrome.WM_KEYUP = 0x101
Chrome.WM_KEYDOWN = 0x100
Chrome.CtrlKey = 0x11
Chrome.NKey = 0x4E

--Functions
-- ToLaunch
function Chrome.ToLaunch()
	LaunchProgram(Chrome.FilePath)
end

function Chrome.ToClose()
	ExitProgram(Chrome.FilePath)
	--ExitStubbornProgram(Pandora.FilePath)
end

-- On Speech Receved
function Chrome.OnSpeechReceved( e )
	if e == "New Tab" then
		--SendMessage( Chrome.FilePath, Chrome.WM_KEYDOWN, Chrome.CtrlKey, 0 )
		--SendMessage( Chrome.FilePath, Chrome.WM_KEYDOWN, Chrome.NKey, 0 )
		--SendMessage( Chrome.FilePath, Chrome.WM_KEYUP, Chrome.NKey, 0 )
		--SendMessage( Chrome.FilePath, Chrome.WM_KEYUP, Chrome.CtrlKey, 0 )
	end
end