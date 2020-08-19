@echo off

dotnet publish .\VRChatRejoinTool.csproj --configuration Release --framework net40 --output .\

if not %errorlevel% == 0 (
	pause
)

del .\VRChatRejoinTool.exe.config