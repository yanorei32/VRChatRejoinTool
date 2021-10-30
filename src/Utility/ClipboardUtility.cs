using System;
using System.Windows.Forms;

namespace VRChatRejoinTool.Utility {
    public static class ClipboardUtility {
        // clipboardを触れるのはGUIスレッド - STAThread - のみ
        [STAThread]
        internal static void copyLaunchInstanceLinkToClipboard(Instance i) {
            Clipboard.SetText(VRChat.GetLaunchInstanceLink(i));
        }

        // clipboardを触れるのはGUIスレッド - STAThread - のみ
        [STAThread]
        internal static void copyInstanceLinkToClipboard(Instance i) {
            Clipboard.SetText(VRChat.GetInstanceLink(i));
        }
    }
}