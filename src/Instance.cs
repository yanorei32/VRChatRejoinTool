using System.Text.RegularExpressions;

class Instance {
	static Regex publicRegex = new Regex(@":\d+$");
	Permission permission;
	string id;

	public Permission Permission {
		get { return this.permission; }
	}

	public string WorldId {
		get { return this.id.Split(':')[0]; }
	}

	public string Id {
		get { return this.id; }
	}

	public Instance(string id) {
		this.id = id;

		this.permission = Permission.Unknown;

		if (id.Contains("canRequestInvite"))
			this.permission = Permission.InvitePlus;
		else if (id.Contains("private"))
			this.permission = Permission.InviteOnly;
		else if (id.Contains("friends"))
			this.permission = Permission.Friends;
		else if (id.Contains("hidden"))
			this.permission = Permission.FriendsPlus;
		else if (id.Contains("public") || publicRegex.IsMatch(id))
			this.permission = Permission.Public;
	}
}

