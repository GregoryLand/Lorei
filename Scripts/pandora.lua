-- Setup Command Line
RegisterLoreiProgramName("Pandora")

-- Setup Keywords
RegisterLoreiProgramCommand("Play")
RegisterLoreiProgramCommand("Pause")
RegisterLoreiProgramCommand("Next")
RegisterLoreiProgramCommand("Skip")
RegisterLoreiProgramCommand("Like")

-- Functions
if( Pandora == nil) then
	Pandora = {}
end

Pandora.FilePath = "C:\\Program Files (x86)\\Pandora\\Pandora.exe"
--Pandora.WM_LBUTTONDOWN = 0x0201
--Pandora.WM_LBUTTONUP = 0x0202
--Pandora.MK_LBUTTON = 0x0001
--Pandora.test = 0x164
Pandora.spacebar = 0x20
Pandora.rightArrowKey = 0x27
Pandora.plusKey = 0x6B
Pandora.WM_KEYUP = 0x101
Pandora.WM_KEYDOWN = 0x100

--ToLaunch
function Pandora.ToLaunch()
	LaunchProgram(Pandora.FilePath)
end

function Pandora.ToClose()
	--Call ExitStubbornProgram because pandora does strange things with messages
	ExitStubbornProgram(Pandora.FilePath)
end

-- On Speech Receved
function Pandora.OnSpeechReceved( e )
	if e == "Play" then
		SendMessage( Pandora.FilePath, Pandora.WM_KEYDOWN, Pandora.spacebar, 0 )
		SendMessage( Pandora.FilePath, Pandora.WM_KEYUP, Pandora.spacebar, 0 )
		end
	if e == "Pause" then
		SendMessage( Pandora.FilePath, Pandora.WM_KEYDOWN, Pandora.spacebar, 0 )
		SendMessage( Pandora.FilePath, Pandora.WM_KEYUP, Pandora.spacebar, 0 )	
		end	
	if e == "Next" then
		SendMessage( Pandora.FilePath, Pandora.WM_KEYDOWN, Pandora.rightArrowKey, 0 )
		SendMessage( Pandora.FilePath, Pandora.WM_KEYUP, Pandora.rightArrowKey, 0 )
		end
	if e == "Skip" then
		SendMessage( Pandora.FilePath, Pandora.WM_KEYDOWN, Pandora.rightArrowKey, 0 )
		SendMessage( Pandora.FilePath, Pandora.WM_KEYUP, Pandora.rightArrowKey, 0 )
		end
	if e == "Like" then
		SendMessage( Pandora.FilePath, Pandora.WM_KEYDOWN, Pandora.plusKey, 0 )
		SendMessage( Pandora.FilePath, Pandora.WM_KEYUP, Pandora.plusKey, 0 )
		end
end