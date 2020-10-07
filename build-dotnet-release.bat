@echo off

dotnet publish ^
	.\VRChatRejoinTool.csproj ^
	--configuration Release ^
	--framework netcoreapp3.1

if not %errorlevel% == 0 (
	pause
)

dotnet publish ^
	.\VRChatRejoinTool.csproj ^
	--configuration Release ^
	--framework net40

if not %errorlevel% == 0 (
	pause
)

