-- Setup Command Line
RegisterLoreiProgramName("pandora");

-- Setup Keywords
RegisterLoreiProgramCommand("Play");

-- Functions
if( pandora== nil) then
	pandora = {}
end

pandora.FilePath = "C:\\Program Files (x86)\\Pandora\\Pandora.exe";
pandora.WM_LBUTTONDOWN = 0x0201;
pandora.WM_LBUTTONUP = 0x0202;
pandora.MK_LBUTTON = 0x0001;
pandora.test = 0x164
--ToLaunch
function pandora.ToLaunch()
	LaunchProgram(pandora.FilePath);
end;

function pandora.ToClose()
	ExitProgram(pandora.FilePath);
end;

-- On Speech Receved
function pandora.OnSpeechReceved( e )
	if e == "Play" then
		SendMessage( pandora.FilePath, pandora.WM_LBUTTONDOWN, pandora.MK_LBUTTON , 9503067);
		SendMessage( pandora.FilePath, pandora.WM_LBUTTONUP,   0 , 1372072);
		end
end;