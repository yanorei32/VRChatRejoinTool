# VRChat Rejoin Tool

This software helps rejoin the instance.

1. Run `VRChatRejoin.exe`
   + Instances are automatically suggested based on the VRChat log file.

![image](https://user-images.githubusercontent.com/11992915/122329570-a1a08000-cf6c-11eb-8356-333e6de43b2c.png)

If you want to use the old log file, read it by drag and drop.

## Latest release
[Download](https://github.com/Yanorei32/VRChatRejoinTool/releases/latest)

[Download from BOOTH (for boost)](https://yanorei32.booth.pm/items/1489700)

## Command-line Arguments

| Argument                             | Description                                                                              |
|:-------------------------------------|:-----------------------------------------------------------------------------------------|
| `--kill-vrc`                         | Kill VRChat.exe processes before launch. (for non-VR)                                    |
| `--invite-me`                        | Use vrc-invite-me.exe integration  (works with `--no-gui` option)                        |
| `--ignore-public`                    | Ignore public and unknown instances.                                                     |
| `--ignore-by-time=<time in minutes>` | Ignore too old visits.                                                                   |
| `--ignore-worlds=wrld_xx,wrld_xx`    | Ignore worlds.                                                                           |
| `--no-gui`                           | No GUI mode (Auto join to first candidate)                                               |
| `--no-dialog`                        | Don't show error dialogs (Sound only). (works with `--no-gui` option.)                   |
| `--quick-save`                       | Quick-save instance shortcut (vrchat) to `AppDir/saves/`  (works with `--no-gui` option) |
| `--quick-save-http`                  | Quick-save instance shortcut (http) to `AppDir/saves/`  (works with `--no-gui` option)   |
| `--index=<n>`                        | Set visit by index (default: 0). (works with `--no-gui` option.)                         |

### Example (VaNii Menu)

https://pastebin.com/axeK0ePs

## Build

### Easily

* Windows 10

1. Download Source Code from [Download ZIP](https://github.com/Yanorei32/VRChatRejoinTool/archive/master.zip) and Extract
1. Run `build-csc.bat`

### Advanced (official)

* Windows 10
* Cygwin or WSL or WSL2 / GNU make

1. Run commands in Cygwin environment:
```bash
git clone https://github.com/yanorei32/VRChatRejoinTool
cd VRChatRejoinTool
make
make genzip # if you need zip file.
```

### dotnet compiler

* dotnet compiler

1. Run `build-dotnet-release.bat`

## Special thanks

* New logo created by [@FUMI23_VRC](https://twitter.com/intent/user?user_id=1217010323695128578)
* Dotnet compiler supported by [@Ram_Type64_Mod0](https://twitter.com/intent/user?user_id=164613634)

