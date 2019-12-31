@echo off

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:winexe ^
	/win32icon:res\icon.ico ^
	/resource:res\icon.ico,icon ^
	/resource:res\logo.png,logo ^
	/out:VRChatRejoin.exe ^
	src\Program.cs ^
	src\Permission.cs ^
	src\Instance.cs ^
	src\Visit.cs ^
	src\VRChat.cs ^
	src\Form\RejoinToolForm.cs ^
	src\Form\MainForm.cs ^
	src\Form\MainForm.Design.cs ^
	src\Form\EditInstanceForm.cs ^
	src\Form\EditInstanceForm.Design.cs

pause

