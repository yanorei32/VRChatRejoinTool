namespace VRChatRejoinTool.Utility {
	public static class LinkGenerator {
	    public static string GetLaunchInstanceLink(Instance i) {
	        return "vrchat://launch?id=" + i.Id;
	    }

	    public static string GetInstanceLink(Instance i) {
	        return string.Format(
	            "https://vrchat.com/home/launch?worldId={0}{1}{2}",
	            i.WorldId,
	            i.IdWithoutWorldId == "" ? "" : "&instanceId=",
	            i.IdWithoutWorldId
	        );
	    }

	    public static string GetUserIdLink(Instance i) {
	        return string.Format(
	            "https://vrchat.com/home/user/{0}",
	            i.OwnerId
	        );
	    }
	}
}
