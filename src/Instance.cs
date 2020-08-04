using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Instance {
	static Regex safeInstanceNameR		= new Regex(@"\A[A-Za-z0-9_\-]+\z");
	static Regex failableInstanceNameR	= new Regex(@"\A[A-Za-z0-9_\-()]+\z");

	static Regex nonceR		= new Regex(@"\A[A-Za-z0-9\-]+\z");
	static Regex userIdR	= new Regex(@"\Ausr_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");
	static Regex worldIdR	= new Regex(@"\Awr?ld_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");

	Permission permission;
	string worldId;
	string ownerId;
	string nonce;
	string instanceName;

	public Permission Permission {
		get { return this.permission; }
		set { this.permission = value; }
	}

	public string OwnerId {
		get { return this.ownerId; }
		set { this.ownerId = value; }
	}

	public string Nonce {
		get { return this.nonce; }
		set { this.nonce = value; }
	}

	public string WorldId {
		get { return this.worldId; }
		set { this.worldId = value; }
	}

	public string InstanceName {
		get { return this.instanceName; }
		set { this.instanceName = value; }
	}

	public string IdWithoutWorldId {
		get {
			if (this.permission == Permission.Unknown)
				return "";

			string id = instanceName;

			if (this.permission == Permission.Public)
				return id;

			id += "~";

			switch (this.permission) {
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

			if (this.ownerId != null)
				id += "(" + this.ownerId + ")";

			if (this.nonce != null)
				id += "~nonce(" + this.nonce + ")";

			return id;
		}
	}

	public string Id {
		get {
			string id = worldId;
			string idWithoutWorldId = this.IdWithoutWorldId;

			if (idWithoutWorldId != "") {
				id += ":";
				id += idWithoutWorldId;
			}

			return id;
		}
	}

	public bool IsMaybeValidInstanceName() {
		return failableInstanceNameR.Match(this.instanceName).Success;
	}

	public bool IsSafeInstanceName() {
		return safeInstanceNameR.Match(this.instanceName).Success;
	}

	public bool IsValidNonceValue() {
		return nonceR.Match(this.nonce).Success;
	}

	public bool IsValidWorldId() {
		return worldIdR.Match(this.worldId).Success;
	}

	public bool IsValidUserId() {
		return userIdR.Match(this.ownerId).Success;
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

		this.permission = Permission.Unknown;

		string[] visibleInfo = splittedId[0].Split(':');

		this.worldId = visibleInfo[0];

		if (visibleInfo.Length != 2) {
			this.permission = Permission.Unknown;
			return;
		}

		this.permission = Permission.Public;
		this.instanceName = visibleInfo[1];

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
					this.nonce = pValue;
					break;

				case "public":
					this.permission = Permission.PublicWithIdentifier;
					break;

				case "private":
					this.permission = Permission.InviteOnly;
					this.ownerId = pValue;
					break;

				case "friends":
					this.permission = Permission.Friends;
					this.ownerId = pValue;
					break;

				case "hidden":
					this.permission = Permission.FriendsPlus;
					this.ownerId = pValue;
					break;

				default:
					break;
			}

		}

		if (containsCanRequestInvite)
			this.permission = Permission.InvitePlus;
	}

	public Instance ShallowCopy() {
		return (Instance) this.MemberwiseClone();
	}

	public Instance(string id) {
		parseId(id);
	}
}

