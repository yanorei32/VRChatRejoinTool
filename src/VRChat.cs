using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VRChatRejoinTool.Utility;

namespace VRChatRejoinTool {
	static class VRChat {
		public static void SaveInstanceToShortcut(Instance i, string filepath, bool httpLink = false) {
			var dynShellType = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
			dynamic shell = Activator.CreateInstance(dynShellType);
			var shortcut = shell.CreateShortcut(filepath);

			shortcut.IconLocation = Application.ExecutablePath + ",0";

			if (!httpLink) {
				shortcut.TargetPath = LinkGenerator.GetLaunchInstanceLink(i);
			} else {
				shortcut.TargetPath = LinkGenerator.GetInstanceLink(i);
			}

			shortcut.Save();
			Marshal.FinalReleaseComObject(shortcut);
			Marshal.FinalReleaseComObject(shell);
		}

		public static void Launch(Instance i, bool killVRC) {
			if (killVRC)
				foreach (var p in Process.GetProcessesByName("vrchat"))
					p.Kill();

			Process.Start(new ProcessStartInfo {
				FileName = LinkGenerator.GetLaunchInstanceLink(i),
				UseShellExecute = true,
			});
		}

		public static int InviteMe(Instance i, string vrcInviteMePath) {
			if (!File.Exists(vrcInviteMePath)) {
				return -1;
			}

			var proc = Process.Start(new ProcessStartInfo {
				FileName = vrcInviteMePath,
				Arguments = LinkGenerator.GetLaunchInstanceLink(i),
				UseShellExecute = true,
			});

			// ReSharper disable once PossibleNullReferenceException
			// アトミックな処理ではないのでそんなに頑張る必要はないとの結論に達した。
			proc.WaitForExit();

			return proc.ExitCode;
		}
	}
}

