using System.Diagnostics;

namespace VRChatRejoinTool.Utility {
    public static class ShellUtility {
        internal static void showDetail(Instance i) {
            openFileWithShell(LinkGenerator.GetInstanceLink(i));
        }

        internal static void showUserDetail(Instance i) {
            openFileWithShell(LinkGenerator.GetUserIdLink(i));
        }
        
        private static void openFileWithShell(string fileName) {
            Process.Start(new ProcessStartInfo {
                FileName = fileName,
                UseShellExecute = true,
            });
        }
    }
}