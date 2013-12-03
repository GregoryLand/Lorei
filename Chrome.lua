--Setup Chrome
RegisterLoreiProgramName("Chrome")

--Setup Chrome keywords
--RegisterLoreiProgramCommand("")

------------ Object containing the Functions required by Lorei ------------
if( chrome == nil) then
	chrome = {}
end

chrome.FilePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"

--Functions
-- ToLaunch
function chrome.ToLaunch()
	LaunchProgram(chrome.FilePath)
end

function chrome.ToClose()
	ExitProgram(chrome.FilePath)
end



-- On Speech Receved
function chrome.OnSpeechReceved( e )
end