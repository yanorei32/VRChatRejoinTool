using System.Diagnostics;

namespace VRChatRejoinTool.Utility {
	public static class ShellUtility {
	    internal static void showDetail(Instance i) {
	        start(LinkGenerator.GetInstanceLink(i));
	    }

	    internal static void showUserDetail(Instance i) {
	        start(LinkGenerator.GetUserIdLink(i));
	    }
	    
	    private static void start(string path) {
	        Process.Start(new ProcessStartInfo {
	            FileName = path,
	            UseShellExecute = true,
	        });
	    }
	}
}
