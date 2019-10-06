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

	public static void VRCWOpen(string worldId) {
		Process.Start("https://www.vrcw.net/world/detail/" + worldId);
	}
}

