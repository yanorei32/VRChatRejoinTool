# VRChat Rejoin Tool

This software helps rejoin the instance.

1. Run `VRChatRejoin.exe`
   + Instances are automatically suggested based on the VRChat log file.

![image](https://user-images.githubusercontent.com/11992915/62416386-d48d4380-b674-11e9-8726-fb7d4b5a7e07.png)

If you want to use the old log file, read it by drag and drop.

[Download](https://github.com/Yanorei32/VRChatRejoinTool/releases)

[Download from BOOTH (for boost)](https://yanorei32.booth.pm/items/1489700)

## Command-line Arguments

| Argument                             | Description                                                                     |
|:-------------------------------------|:--------------------------------------------------------------------------------|
| `--kill-vrc`                         | Kill VRChat.exe processes before launch. (for non-VR)                           |
| `--ignore-public`                    | Ignore public and unknown instances.                                            |
| `--ignore-by-time=<time in minutes>` | Ignore too old visits.                                                          |
| `--ignore-worlds=wrld_xx,wrld_xx`    | Ignore worlds.                                                                  |
| `--no-gui`                           | No GUI mode (Auto join to first candidate)                                      |
| `--no-dialog`                        | Don't show error dialogs (Sound only). (works with `--no-gui` option.)          |
| `--quick-save`                       | Quick-save instance shortcut to `AppDir/saves/`  (works with `--no-gui` option) |

### Example

#### Launch from CUI

```bat
VRChatRejoin.exe ^
	--kill-vrc ^
	--ignore-public ^
	--ignore-by-time=540 ^
	--no-gui
```

#### Launch from VaniiMenu Launcher

```json
"App1": {
	"arguments": "--no-gui --no-dialog --ignore-by-time=540"
},
"App2": {
	"arguments": "--no-gui --no-dialog --ignore-by-time=540 --ignore-public"
}
"App3": {
	"arguments": "--no-gui --no-dialog --quick-save"
}
```

## Build

* Windows 10
* Cygwin / GNU make

```bash
git clone https://github.com/yanorei32/VRChatRejoinTool
cd VRChatRejoinTool
make
make genzip # if you need zip file.
```

