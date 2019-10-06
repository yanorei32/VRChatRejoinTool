using System.Diagnostics;

static class VRChat {
	public static void KillExistProcesses() {
		foreach (var p in Process.GetProcessesByName("vrchat"))
			p.Kill();
	}

	public static void Launch(string instanceId) {
		Process.Start("vrchat://launch?id=" + instanceId);
	}

	public static void VRCWOpen(string worldId) {
		Process.Start("https://www.vrcw.net/world/detail/" + worldId);
	}
}

