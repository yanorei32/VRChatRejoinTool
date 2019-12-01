using System.Diagnostics;

static class VRChat {
	private static void killExistProcesses() {
		foreach (var p in Process.GetProcessesByName("vrchat"))
			p.Kill();
	}

	public static void Launch(string instanceId, bool killVRC) {
		if (killVRC)
			killExistProcesses();

		Process.Start("vrchat://launch?id=" + instanceId);
	}

	public static void OpenVRCW(string worldId) {
		Process.Start("https://www.vrcw.net/world/detail/" + worldId);
	}

	public static void OpenOfficialSite(string worldId) {
		Process.Start("https://vrchat.com/home/world/" + worldId);
	}
}

