# Run this script before execute build-csc.bat
$ls = ls -R -File "src" -Filter '**.cs' `
| % { "`t" + $_.Fullname.replace($pwd.Path + "`\", "") }
$buildCSC = @"
@echo off
@rem run Bootstrap.bat before this script.

mkdir bin\csc\

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:winexe ^
	/win32icon:res\icon.ico ^
	/resource:res\icon.ico,icon ^
	/resource:res\logo.png,logo ^
	/out:bin\csc\VRChatRejoinTool.exe ^
	/nologo ^
	/optimize ^
$($ls -join " ^`n")

if not %errorlevel% == 0 (
	pause
)
"@
$buildCSC | Set-Content build-csc.bat