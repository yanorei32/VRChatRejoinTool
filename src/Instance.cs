using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Instance {
	static Regex safeInstanceNameR		= new Regex(@"\A[A-Za-z0-9_\-]+\z");
	static Regex failableInstanceNameR	= new Regex(@"\A[A-Za-z0-9_\-()]+\z");

	static Regex nonceR		= new Regex(@"\A[A-Za-z0-9\-]+\z");
	static Regex userIdR	= new Regex(@"\Ausr_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");
	static Regex worldIdR	= new Regex(@"\Awr?ld_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");

    public Permission Permission { get; set; }

    public string OwnerId { get; set; }

    public string Nonce { get; set; }

    public string WorldId { get; set; }

    public string InstanceName { get; set; }

    public string IdWithoutWorldId {
		get {
			if (this.Permission == Permission.Unknown)
				return "";

			string id = InstanceName;

			if (this.Permission == Permission.Public)
				return id;

			id += "~";

			switch (this.Permission) {
				case Permission.PublicWithIdentifier:
					id += "public";
					break;

				case Permission.FriendsPlus:
					id += "hidden";
					break;

				case Permission.Friends:
					id += "friends";
					break;

				case Permission.InvitePlus:
					id += "canRequestInvite~private";
					break;

				case Permission.InviteOnly:
					id += "private";
					break;
			}

			if (this.OwnerId != null)
				id += "(" + this.OwnerId + ")";

			if (this.Nonce != null)
				id += "~nonce(" + this.Nonce + ")";

			return id;
		}
	}

	public string Id {
		get {
			string id = WorldId;
			string idWithoutWorldId = this.IdWithoutWorldId;

			if (idWithoutWorldId != "") {
				id += ":";
				id += idWithoutWorldId;
			}

			return id;
		}
	}

	public bool IsMaybeValidInstanceName() {
		return failableInstanceNameR.Match(this.InstanceName).Success;
	}

	public bool IsSafeInstanceName() {
		return safeInstanceNameR.Match(this.InstanceName).Success;
	}

	public bool IsValidNonceValue() {
		return nonceR.Match(this.Nonce).Success;
	}

	public bool IsValidWorldId() {
		return worldIdR.Match(this.WorldId).Success;
	}

	public bool IsValidUserId() {
		return userIdR.Match(this.OwnerId).Success;
	}

	void parseId(string id) {
		// NOTE:
		//   instanceName isn't contains ':'
		//   nonce, instanceName isn't contains '~'
		//   non-invite+ instances isn't contains
		//     "canRequestInvite" parameter
		//   all non-home instances has instance-name
		//   Is valid? wrld_xx~aa
		//
		//   ほんまか？

		string[] splittedId = id.Split('~');

		this.Permission = Permission.Unknown;

		string[] visibleInfo = splittedId[0].Split(':');

		this.WorldId = visibleInfo[0];

		if (visibleInfo.Length != 2) {
			this.Permission = Permission.Unknown;
			return;
		}

		this.Permission = Permission.Public;
		this.InstanceName = visibleInfo[1];

		bool containsCanRequestInvite = false;
		for (int i = 1; i < splittedId.Length; i++) {
			if (splittedId[i] == "canRequestInvite") {
				containsCanRequestInvite = true;
				continue;
			}

			string pKey, pValue;

			{
				string[] splittedParam = splittedId[i].Split('(');

				if (2 < splittedParam.Length) continue;

				pKey = splittedParam[0];

				if (splittedParam.Length == 2) {
					pValue = splittedParam[1].Substring(0, splittedParam[1].Length-1);
					if (pValue == "") pValue = null;
				} else {
					pValue = null;
				}
			}

			switch (pKey) {
				case "nonce":
					this.Nonce = pValue;
					break;

				case "public":
					this.Permission = Permission.PublicWithIdentifier;
					break;

				case "private":
					this.Permission = Permission.InviteOnly;
					this.OwnerId = pValue;
					break;

				case "friends":
					this.Permission = Permission.Friends;
					this.OwnerId = pValue;
					break;

				case "hidden":
					this.Permission = Permission.FriendsPlus;
					this.OwnerId = pValue;
					break;

				default:
					break;
			}

		}

		if (containsCanRequestInvite)
			this.Permission = Permission.InvitePlus;
	}

	public Instance ShallowCopy() {
		return (Instance) this.MemberwiseClone();
	}

	public Instance(string id) {
		parseId(id);
	}
}

