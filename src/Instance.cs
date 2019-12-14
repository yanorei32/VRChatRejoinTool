using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Instance {
	/* VRChat logging style exception message */
	const string EXCEPTION_BASE_TEXT = "Something Very Bad Happend: ";
	static Regex safeInstanceNameR = new Regex(@"\A[A-Za-z0-9]+\z");
	static Regex maybeInstanceNameR = new Regex(@"\A[^ :~]+\z");
	static Regex nonceR = new Regex(@"\A[A-Za-z\-]+\z");
	static Regex userIdR = new Regex(@"\Ausr_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");
	static Regex worldIdR = new Regex(@"\Awrld_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");

	string rawId;

	Permission permission;
	string ownerId;
	string worldId;
	string nonce;
	string instanceName;

	public Permission Permission {
		get { return this.permission; }
		set {
			rawId = null;
			this.permission = value;
		}
	}

	public string OwnerId {
		get { return this.ownerId; }
		set {
			rawId = null;
			this.ownerId = value;
		}
	}

	public string Nonce {
		get { return this.nonce; }
		set {
			rawId = null;
			this.nonce = value;
		}
	}

	public string WorldId {
		get { return this.worldId; }
		set {
			rawId = null;
			this.worldId = value;
		}
	}

	public string RawId {
		get {
			return this.rawId;
		}
	}

	public string IdWithoutWorldId {
		get {
			if (this.permission == Permission.Unknown)
				return "";

			string id = instanceName;

			if (this.ownerId != null) {
				id += "~";

				switch (this.permission) {
					case Permission.Public:
						id += "public";
						break;

					case Permission.FriendsPlus:
						id += "hidden";
						break;

					case Permission.Friends:
						id += "friends";
						break;

					case Permission.InviteOnly:
						id += "canRequestInvite~invite";
						break;

					case Permission.InvitePlus:
						id += "invite";
						break;
				}

				if (this.ownerId != null)
					id += "(" + this.ownerId + ")";
			}

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

	private bool isValidInstanceName (string instanceName) {
		return maybeInstanceNameR.Match(instanceName).Success;
	}

	private bool isSafeInstanceName (string instanceName) {
		return safeInstanceNameR.Match(instanceName).Success;
	}

	private bool isValidNonceValue (string nonce) {
		return nonceR.Match(nonce).Success;
	}

	private bool isValidWorldId (string worldId) {
		return worldIdR.Match(worldId).Success;
	}

	private bool isValidUserId (string userId) {
		return userIdR.Match(userId).Success;
	}

	public List<InputError> Errors {
		get {
			List<InputError> errors = new List<InputError>();

			if (!isValidUserId(this.ownerId))
				errors.Add(new InputError("Invalid owner id", InputErrorLevel.Error));

			if (!isValidWorldId(this.worldId))
				errors.Add(new InputError("Invalid world id", InputErrorLevel.Error));

			if (!this.IsUniqueInstance)
				errors.Add(new InputError("Instance is not unique", InputErrorLevel.Warning));

			if (this.nonce != null && !isValidNonceValue(this.nonce))
				errors.Add(new InputError("Invalid nonce value", InputErrorLevel.Error));

			if (!isSafeInstanceName(this.instanceName))
				if (!isValidInstanceName(this.instanceName))
					errors.Add(new InputError("Invalid instance name", InputErrorLevel.Error));
				else
					errors.Add(
						new InputError(
							"Instance name has unknown char. "
							+ "Sometimes, It's invalid instance name and sometimes makes buggy client.",
							InputErrorLevel.Warning
						)
					);

			return errors;
		}
	}

	public bool IsUniqueInstance {
		get {
			switch (this.permission) {
				case Permission.Public:
					return (this.instanceName != null);

				case Permission.InviteOnly:
				case Permission.InvitePlus:
				case Permission.Friends:
				case Permission.FriendsPlus:
					return (this.instanceName != null && this.ownerId != null && this.nonce != null);

				default:
					return false;
			}
		}
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
					this.permission = Permission.Public;
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

	public Instance(string id) {
		parseId(id);
		this.rawId = id;
	}
}

