using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

static class VRChat {
	public static void SaveInstanceToShortcut(Instance i, string filepath, bool httpLink = false) {
		var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
		dynamic shell = Activator.CreateInstance(t);
		var shortcut = shell.CreateShortcut(filepath);

		shortcut.IconLocation = Application.ExecutablePath + ",0";

		if (!httpLink) {
			shortcut.TargetPath = VRChat.GetLaunchInstanceLink(i);
		} else {
			shortcut.TargetPath = VRChat.GetInstanceLink(i);
		}

		shortcut.Save();
		Marshal.FinalReleaseComObject(shortcut);
		Marshal.FinalReleaseComObject(shell);
	}

	public static string GetLaunchInstanceLink(Instance i) {
		return "vrchat://launch?id=" + i.Id;
	}

	public static string GetInstanceLink(Instance i) {
		return string.Format(
			"https://vrchat.com/home/launch?worldId={0}&instanceId={1}",
			i.WorldId,
			i.IdWithoutWorldId
		);
	}

	public static string GetUserIdLink(Instance i) {
		return string.Format(
			"https://vrchat.com/home/user/{0}",
			i.OwnerId
		);
	}

	public static void Launch(Instance i, bool killVRC) {
		if (killVRC)
			foreach (var p in Process.GetProcessesByName("vrchat"))
				p.Kill();

		Process.Start(new ProcessStartInfo() {
			FileName = GetLaunchInstanceLink(i),
			UseShellExecute = true,
		});
	}

	public static int InviteMe(Instance i, string vrcInviteMePath) {
		if (!File.Exists(vrcInviteMePath)) {
			return -1;
		}

		var proc = Process.Start(new ProcessStartInfo() {
			FileName = vrcInviteMePath,
			Arguments = GetLaunchInstanceLink(i),
			UseShellExecute = true,
		});

		proc.WaitForExit();

		return proc.ExitCode;
	}
}

