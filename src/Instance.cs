using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Instance {
	static Regex safeInstanceNameR		= new Regex(@"\A[A-Za-z0-9_\-]+\z");
	static Regex failableInstanceNameR	= new Regex(@"\A[A-Za-z0-9_\-()]+\z");

	static Regex nonceR		= new Regex(@"\A[A-Za-z0-9\-]+\z");
	static Regex userIdR	= new Regex(@"\Ausr_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");
	static Regex worldIdR	= new Regex(@"\Awr?ld_[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}\z");

	public InstanceArgument[] ArgumentOrder { get; set; }

	public Permission Permission { get; set; }

	public string WorldName { get; set; }

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

			for (int i = 0; i < ArgumentOrder.Length; i++ ) {
				switch (ArgumentOrder[i]) {
					case InstanceArgument.Permission:
						switch (this.Permission) {
							case Permission.PublicWithIdentifier:
								id += "~public";
								break;

							case Permission.FriendsPlus:
								id += "~hidden";
								break;

							case Permission.Friends:
								id += "~friends";
								break;

							case Permission.InvitePlus:
							case Permission.InviteOnly:
								id += "~private";
								break;
						}

						if (this.OwnerId != null)
							id += "(" + this.OwnerId + ")";

						break;

					case InstanceArgument.Nonce:
						if (this.Nonce != null)
							id += "~nonce(" + this.Nonce + ")";

						break;

					case InstanceArgument.CanRequestInvite:
						if (this.Permission == Permission.InvitePlus)
							id += "~canRequestInvite";

						break;
				}
			}

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
		//	 "canRequestInvite" parameter
		//   all non-home instances has instance-name
		//   Is valid? wrld_xx~aa
		//
		//   ほんまか？

		var argumentPositions = new SortedDictionary<InstanceArgument, int>();
		argumentPositions.Add(InstanceArgument.Permission, -1);
		argumentPositions.Add(InstanceArgument.CanRequestInvite, -1);
		argumentPositions.Add(InstanceArgument.Nonce, -1);

		string[] splittedId = id.Split('~');

		this.Permission = Permission.Unknown;

		string[] visibleInfo = splittedId[0].Split(':');

		this.WorldId = visibleInfo[0];

		if (visibleInfo.Length != 2) {
			this.Permission = Permission.Unknown;

			ArgumentOrder[0] = InstanceArgument.Permission;
			ArgumentOrder[1] = InstanceArgument.CanRequestInvite;
			ArgumentOrder[2] = InstanceArgument.Nonce;

			return;
		}

		this.Permission = Permission.Public;
		this.InstanceName = visibleInfo[1];

		bool containsCanRequestInvite = false;


		for (int i = 1; i < splittedId.Length; i++) {
			if (splittedId[i] == "canRequestInvite") {
				argumentPositions[InstanceArgument.CanRequestInvite] = i * 10;
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
					argumentPositions[InstanceArgument.Nonce] = i * 10;
					this.Nonce = pValue;
					break;

				case "public":
					argumentPositions[InstanceArgument.Permission] = i * 10;
					this.Permission = Permission.PublicWithIdentifier;
					break;

				case "private":
					argumentPositions[InstanceArgument.Permission] = i * 10;
					this.Permission = Permission.InviteOnly;
					this.OwnerId = pValue;
					break;

				case "friends":
					argumentPositions[InstanceArgument.Permission] = i * 10;
					this.Permission = Permission.Friends;
					this.OwnerId = pValue;
					break;

				case "hidden":
					argumentPositions[InstanceArgument.Permission] = i * 10;
					this.Permission = Permission.FriendsPlus;
					this.OwnerId = pValue;
					break;

				default:
					break;
			}
		}

		if (argumentPositions[InstanceArgument.Permission] == -1)
			argumentPositions[InstanceArgument.Permission] = 0;

		if (argumentPositions[InstanceArgument.CanRequestInvite] == -1)
			argumentPositions[InstanceArgument.CanRequestInvite]
				= argumentPositions[InstanceArgument.Permission] + 1;

		if (argumentPositions[InstanceArgument.Nonce] == -1)
			argumentPositions[InstanceArgument.Nonce]
				= argumentPositions[InstanceArgument.CanRequestInvite] + 1;

		int j = 0;
		foreach (
			KeyValuePair<InstanceArgument, int> argument
				in argumentPositions.OrderBy(v => v.Value)
		) {
			ArgumentOrder[j++] = argument.Key;
		}

		if (containsCanRequestInvite)
			this.Permission = Permission.InvitePlus;
	}

	public Instance ShallowCopy() {
		return (Instance) this.MemberwiseClone();
	}

	public Instance(string id, string worldName) {
		ArgumentOrder = new InstanceArgument[3];
		parseId(id);
		this.WorldName = worldName;
	}
}

