using System;

[Flags]
internal enum LogParseState {
	// ビット演算をしている箇所があり、0b00はclearな値であるため各エントリに明示的に値を与える
	Cleared = 0b00,
	DestinationSetFound = 0b01,
	WorldNameFound = 0b10,
}
