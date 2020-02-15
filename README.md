# VRChat Rejoin Tool

This software helps rejoin the instance.

1. Run `VRChatRejoin.exe`
   + Instances are automatically suggested based on the VRChat log file.

![image](https://user-images.githubusercontent.com/11992915/74582703-0afa3280-4fb7-11ea-8809-5f9f5f432f7f.png)

If you want to use the old log file, read it by drag and drop.

[Download](https://github.com/Yanorei32/VRChatRejoinTool/releases)

[Download from BOOTH (for boost)](https://yanorei32.booth.pm/items/1489700)

## Command-line Arguments

| Argument                             | Description                                                                              |
|:-------------------------------------|:-----------------------------------------------------------------------------------------|
| `--kill-vrc`                         | Kill VRChat.exe processes before launch. (for non-VR)                                    |
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
1. Run `compile.bat`


### Advanced (official)

* Windows 10
* Cygwin / GNU make

1. Run commands in Cygwin environment:
```bash
git clone https://github.com/yanorei32/VRChatRejoinTool
cd VRChatRejoinTool
make
make genzip # if you need zip file.
```

