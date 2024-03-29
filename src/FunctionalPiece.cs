using System;
using System.Text.RegularExpressions;

/**
* ユーティリティー関数を集めたネームスペース
*/
namespace VRChatRejoinTool { 
	public static class FunctionalPiece {
		/* 
		* <summary><c>input</c>が<c>regex</c>にマッチしたときのみ<c>ifMatch</c>で指定された<c>Action</c>を呼び出す。</summary>
		* <returns>マッチしてアクションを実行した場合は<c>true</c>、そうでないばあいは<c>false</c></returns>
		*/
		internal static bool ActionIfMatch(string input, Regex regex, Action ifMatch) {
			var m = regex.Match(input);
			if (!m.Success) return false;
			ifMatch();
			return true;
		}

		internal static bool ActionIfMatch(string input, Regex regex, Action<string> matchAction) {
			var m = regex.Match(input);
			if (!m.Success) return false;
			matchAction(m.Value);
			return true;
		}
	}
}
