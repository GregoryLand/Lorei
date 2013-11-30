-- Setup Command Line
RegisterLoreiProgramName("pandora");

-- Setup Keywords
RegisterLoreiProgramCommand("Play");
RegisterLoreiProgramCommand("Pause");
RegisterLoreiProgramCommand("Skip");

-- Functions
if( pandora== nil) then
	pandora = {}
end

pandora.FilePath = "C:\\Program Files (x86)\\Pandora\\Pandora.exe";
--pandora.WM_LBUTTONDOWN = 0x0201;
--pandora.WM_LBUTTONUP = 0x0202;
--pandora.MK_LBUTTON = 0x0001;
--pandora.test = 0x164
pandora.spacebar = 0x20;
pandora.rightArrowKey = 0x27;
pandora.WM_KEYUP = 0x101;
pandora.WM_KEYDOWN = 0x100;

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
		SendMessage( pandora.FilePath, pandora.WM_KEYDOWN, pandora.spacebar, 0 );
		SendMessage( pandora.FilePath, pandora.WM_KEYUP, pandora.spacebar, 0 );
		end
	if e == "Pause" then
		SendMessage( pandora.FilePath, pandora.WM_KEYDOWN, pandora.spacebar, 0 );
		SendMessage( pandora.FilePath, pandora.WM_KEYUP, pandora.spacebar, 0 );	
		end	
	if e == "Next" then
		SendMessage( pandora.FilePath, pandora.WM_KEYDOWN, pandora.rightArrowKey, 0 );
		SendMessage( pandora.FilePath, pandora.WM_KEYUP, pandora.rightArrowKey, 0 );
		end
end;