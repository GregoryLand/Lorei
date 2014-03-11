-- Setup Command Line
RegisterLoreiProgramName("VLC")

-- Setup Keywords
RegisterLoreiProgramCommand("Play")
RegisterLoreiProgramCommand("Pause")
RegisterLoreiProgramCommand("Fullscreen")
RegisterLoreiProgramCommand("Windowed")
RegisterLoreiProgramCommand("Next")


-- Functions
if( VLC == nil) then
	VLC = {}
end

VLC.FilePath = "C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe"
--VLC.WM_LBUTTONDOWN = 0x0201
--VLC.WM_LBUTTONUP = 0x0202
--VLC.MK_LBUTTON = 0x0001
--VLC.test = 0x164
VLC.spacebar = 0x20
VLC.rightArrowKey = 0x27
VLC.plusKey = 0x6B
VLC.WM_KEYUP = 0x101
VLC.WM_KEYDOWN = 0x100
VLC.FKey = 0x46
VLC.NKey = 0x4E
VLC.ESC  = 0x1B

--ToLaunch
function VLC.ToLaunch()
	LaunchProgram(VLC.FilePath)
end

function VLC.ToClose()
	ExitProgram(VLC.FilePath)
end

-- On Speech Receved
function VLC.OnSpeechReceved( e )
	if e == "Play" then
		SendMessage( VLC.FilePath, VLC.WM_KEYDOWN, VLC.spacebar, 0 )
		SendMessage( VLC.FilePath, VLC.WM_KEYUP, VLC.spacebar, 0 )
		end
	if e == "Pause" then
		SendMessage( VLC.FilePath, VLC.WM_KEYDOWN, VLC.spacebar, 0 )
		SendMessage( VLC.FilePath, VLC.WM_KEYUP, VLC.spacebar, 0 )	
		end	
	if e == "Fullscreen" then
		SendMessage( VLC.FilePath, VLC.WM_KEYDOWN, VLC.FKey, 0 )
		SendMessage( VLC.FilePath, VLC.WM_KEYUP, VLC.FKey, 0 )
		end
	if e == "Windowed" then
		SendMessage( VLC.FilePath, VLC.WM_KEYDOWN, VLC.ESC, 0 )
		SendMessage( VLC.FilePath, VLC.WM_KEYUP, VLC.ESC, 0 )
		end
	if e == "Next" then
		SendMessage( VLC.FilePath, VLC.WM_KEYDOWN, VLC.NKey, 0 )
		SendMessage( VLC.FilePath, VLC.WM_KEYUP, VLC.NKey, 0 )
		end
end
