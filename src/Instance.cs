using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Instance {
	static Regex instanceNameR	= new Regex(@"\A[A-Za-z0-9]+\z");
	static Regex nonceR			= new Regex(@"\A([0-9A-F]{48}|[0-9A-F]{64}|[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[a-f0-9]{4}-[a-f0-9]{12})\z");
	static Regex userIdR		= new Regex(@"\Ausr_[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[a-f0-9]{4}-[a-f0-9]{12}\z");
	static Regex worldIdR		= new Regex(@"\Awr?ld_[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[a-f0-9]{4}-[a-f0-9]{12}\z");
	static Regex regionR		= new Regex(@"\A[a-z]{2}\z");

	public InstanceArgument[] ArgumentOrder { get; set; }

	public Permission Permission { get; set; }

	public ServerRegion Region { get; set; }
	public string CustomRegion { get; set; }

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

			for (int i = 0; i < ArgumentOrder.Length; i++ ) {
				switch (ArgumentOrder[i]) {
					case InstanceArgument.Region:
						switch (this.Region) {
							case ServerRegion.JP:
								id += "~region(jp)";
								break;

							case ServerRegion.EU:
								id += "~region(eu)";
								break;

							case ServerRegion.US:
								id += "";
								break;

							case ServerRegion.Custom:
								id += "~region(" + this.CustomRegion + ")";
								break;
						}

						break;

					case InstanceArgument.Permission:
						if (this.Permission == Permission.Public)
							break;

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

						if (this.Permission != Permission.PublicWithIdentifier)
							if (this.OwnerId != null)
								id += "(" + this.OwnerId + ")";

						break;

					case InstanceArgument.Nonce:
						if (this.Permission != Permission.PublicWithIdentifier)
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

	public string RegionName {
		get {
			switch (this.Region) {
				case ServerRegion.US:
					return "US";
				case ServerRegion.EU:
					return "EU";
				case ServerRegion.JP:
					return "JP";
				default:
					return this.CustomRegion.ToUpper();
			}
		}
	}

	public bool IsValidCustomRegionName() {
		return regionR.Match(this.CustomRegion).Success;
	}

	public bool IsValidInstanceName() {
		return instanceNameR.Match(this.InstanceName).Success;
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

	public bool IsValidPermission() {
		return this.Permission != Permission.PublicWithIdentifier;
	}

	public bool IsValidArgumentOrder() {
		var reference = new InstanceArgument[4];
		reference[0] = InstanceArgument.Permission;
		reference[1] = InstanceArgument.CanRequestInvite;
		reference[2] = InstanceArgument.Region;
		reference[3] = InstanceArgument.Nonce;

		for (int i = 0; i < reference.Length; ++i)
			if (this.ArgumentOrder[i] != reference[i])
				return false;

		return true;
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
		argumentPositions.Add(InstanceArgument.Region, -1);
		argumentPositions.Add(InstanceArgument.Nonce, -1);

		string[] splittedId = id.Split('~');

		this.Permission = Permission.Unknown;
		this.Region = ServerRegion.US;
		this.CustomRegion = "";

		string[] visibleInfo = splittedId[0].Split(':');

		this.WorldId = visibleInfo[0];

		if (visibleInfo.Length != 2) {
			this.Permission = Permission.Unknown;

			ArgumentOrder[0] = InstanceArgument.Permission;
			ArgumentOrder[1] = InstanceArgument.CanRequestInvite;
			ArgumentOrder[2] = InstanceArgument.Region;
			ArgumentOrder[3] = InstanceArgument.Nonce;

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
				case "region":
					argumentPositions[InstanceArgument.Region] = i * 10;

					switch (pValue) {
						case "eu":
							this.Region = ServerRegion.EU;
							break;

						case "jp":
							this.Region = ServerRegion.JP;
							break;

						default:
							this.Region = ServerRegion.Custom;
							this.CustomRegion = pValue;
							break;
					}

					break;

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

		if (argumentPositions[InstanceArgument.Region] == -1)
			argumentPositions[InstanceArgument.Region]
				= argumentPositions[InstanceArgument.CanRequestInvite] + 1;

		if (argumentPositions[InstanceArgument.Nonce] == -1)
			argumentPositions[InstanceArgument.Nonce]
				= argumentPositions[InstanceArgument.Region] + 1;

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
		ArgumentOrder = new InstanceArgument[4];
		parseId(id);
		this.WorldName = worldName;
	}
}

