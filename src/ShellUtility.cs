using System.Diagnostics;

namespace VRChatRejoinTool {
    public static class ShellUtility {
        internal static void showDetail(Instance i) {
            openFileWithShell(VRChat.GetInstanceLink(i));
        }

        internal static void showUserDetail(Instance i) {
            openFileWithShell(VRChat.GetUserIdLink(i));
        }
        
        private static void openFileWithShell(string fileName) {
            Process.Start(new ProcessStartInfo {
                FileName = fileName,
                UseShellExecute = true,
            });
        }
    }
}