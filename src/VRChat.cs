using System.Diagnostics;

static class VRChat {
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

	public static void Launch(Instance i, bool killVRC) {
		if (killVRC)
			foreach (var p in Process.GetProcessesByName("vrchat"))
				p.Kill();

		Process.Start(GetLaunchInstanceLink(i));
	}
}

