using System;
using System.Windows.Forms;

namespace VRChatRejoinTool.Utility {
	public static class ClipboardUtility {
		internal static void CopyLaunchInstanceLinkToClipboard(Instance i) {
			Clipboard.SetText(LinkGenerator.GetLaunchInstanceLink(i));
		}

		internal static void CopyInstanceLinkToClipboard(Instance i) {
			Clipboard.SetText(LinkGenerator.GetInstanceLink(i));
		}
	}
}
