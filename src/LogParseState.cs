using System;

namespace VRChatRejoinTool {
	[Flags]
	internal enum LogParseState {
		// ビット演算をしている箇所があり、0b00はclearな値であるため各エントリに明示的に値を与える
		Cleared = 0x00,
		DestinationSetFound = 0x01,
		WorldNameFound = 0x02,
	}
}
