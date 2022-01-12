@echo off

mkdir bin\csc\

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:winexe ^
	/win32icon:res\icon.ico ^
	/resource:res\icon.ico,icon ^
	/resource:res\logo.png,logo ^
	/out:bin\csc\VRChatRejoinTool.exe ^
	/nologo ^
	/optimize ^
	src\FunctionalPiece.cs ^
	src\Instance.cs ^
	src\LogParseState.cs ^
	src\Permission.cs ^
	src\Program.cs ^
	src\ServerRegion.cs ^
	src\Visit.cs ^
	src\VRChat.cs ^
	src\Form\EditInstanceForm.cs ^
	src\Form\EditInstanceForm.Design.cs ^
	src\Form\MainForm.cs ^
	src\Form\MainForm.Design.cs ^
	src\Utility\ClipboardUtility.cs ^
	src\Utility\LinkGenerator.cs ^
	src\Utility\SaveInstanceUtility.cs ^
	src\Utility\ShellUtility.cs

if not %errorlevel% == 0 (
	pause
)

