﻿using System;
using System.Windows.Forms;

namespace VRChatRejoinTool.Utility {
	public static class ClipboardUtility {
	    internal static void copyLaunchInstanceLinkToClipboard(Instance i) {
	        Clipboard.SetText(LinkGenerator.GetLaunchInstanceLink(i));
	    }

	    internal static void copyInstanceLinkToClipboard(Instance i) {
	        Clipboard.SetText(LinkGenerator.GetInstanceLink(i));
	    }
	}
}