!include "MUI2.nsh"

; General
  Name "VRChatRejoinTool"
  OutFile "VRChatRejoinTool.Installer.exe"
  Unicode True

  InstallDir "$LOCALAPPDATA\VRChatRejoinTool"
  InstallDirRegKey HKCU "Software\VRChatRejoinTool" ""

  RequestExecutionLevel user

; Compress
  SetCompressor /SOLID lzma
  SetDatablockOptimize ON

; Interface
  !define MUI_ABORTWARNING¬

; Pages
  !define MUI_ICON "res\icon.ico"
  !define MUI_UNICON "res\icon.ico"
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "res\installer-logo.bmp"
  !define MUI_HEADERIMAGE_RIGHT

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "LICENSE"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH

; Registry
  !define REGPATH_UNINSTSUBKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\VRChatRejoinTool"

; Languages
  !insertmacro MUI_LANGUAGE "English"

;Installer Sections
Section "VRChat RejoinTool" SecProgram
  SectionIn RO
  SetOutPath "$INSTDIR"

  File "bin\csc\VRChatRejoinTool.exe"
  CreateShortcut "$SMPROGRAMS\VRChat RejoinTool.lnk" "$INSTDIR\VRChatRejoinTool.exe"

  ;Store installation folder
  WriteRegStr HKCU "Software\VRChatRejoinTool" "" $INSTDIR

  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "DisplayName" "VRChat RejoinTool"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "Publisher" "yanorei32"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "Readme" "https://github.com/yanorei32/VRChatRejoinTool"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "URLUpdateInfo" "https://github.com/yanorei32/VRChatRejoinTool/releases"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "URLInfoAbout" "https://github.com/yanorei32/VRChatRejoinTool"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "HelpLink" "https://twitter.com/yanorei32"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "Comments" "This software helps rejoin to the VRChat instance."
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "DisplayIcon" "$INSTDIR\VRChatRejoinTool.exe,0"
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegStr HKCU "${REGPATH_UNINSTSUBKEY}" "QuietUninstallString" '"$INSTDIR\Uninstall.exe" /S'
  WriteRegDWORD HKCU "${REGPATH_UNINSTSUBKEY}" "EstimatedSize" 192
  WriteRegDWORD HKCU "${REGPATH_UNINSTSUBKEY}" "NoModify" 1
  WriteRegDWORD HKCU "${REGPATH_UNINSTSUBKEY}" "NoRepair" 1
SectionEnd

; Uninstaller
Section "Uninstall"
  Delete "$INSTDIR\Uninstall.exe"
  Delete "$SMPROGRAMS\VRChat RejoinTool.lnk"
  Delete "$INSTDIR\VRChatRejoinTool.exe"
  RMDir "$INSTDIR"

  DeleteRegKey HKCU "${REGPATH_UNINSTSUBKEY}"
SectionEnd

