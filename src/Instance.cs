using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VRChatRejoinTool {
	public class Instance {
		static Regex instanceNameR	= new Regex(@"\A[A-Za-z0-9]+\z");
		static Regex nonceR			= new Regex(@"\A([0-9A-F]{48}|[0-9A-F]{64}|[a-f0-9]{8}-[a-f0-9]{4}-[0-5][a-f0-9]{3}-[a-b0-9][a-f0-9]{3}-[a-f0-9]{12})\z");
		static Regex userIdR		= new Regex(@"\Ausr_[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[a-f0-9]{4}-[a-f0-9]{12}\z");
		static Regex worldIdR		= new Regex(@"\Awr?ld_[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}\z");
		static Regex regionR		= new Regex(@"\A[a-z]{1,3}\z");
	
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
				

				string id = InstanceName;

			switch (this.Permission) {
				case Permission.Unknown:
					return "";

				case Permission.Public:
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

							if (this.Permission != Permission.Public)
								if (this.OwnerId != null)
									id += "(" + this.OwnerId + ")";

				if (this.Permission == Permission.InvitePlus)
				id += "~canRequestInvite";

			switch (this.Region) {
				case ServerRegion.USW:
					id += "";
					break;

						case ServerRegion.USWWithIdentifier:
								id += "~region(us)";
									break;

					case ServerRegion.USE:
					id += "~region(use)";
					break;

						case ServerRegion.JP:
								id += "~region(jp)";
					break;

					case ServerRegion.EU:
					id += "~region(eu)";
					break;

				case ServerRegion.Custom:
					id += "~region(" + this.CustomRegion + ")";		break;
					}
				if (this.Permission != Permission.Public)
				if (this.Nonce != null)
					id += "~nonce(" + this.Nonce + ")";

				return id;
			}
		}

		public string Id {
			get {
				string id = WorldId;
				string idWithoutWorldId = this.IdWithoutWorldId;

				if (idWithoutWorldId == "") return id;
				id += ":";
				id += idWithoutWorldId;

				return id;
			}
		}

		public string RegionName {
			get {
				switch (this.Region) {
					case ServerRegion.USWWithIdentifier:
					case ServerRegion.USW:
						return "USW";
				case ServerRegion.USE:
					return "USE";
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

		// NOTE: splitは過去分詞
		string[] splitId = id.Split('~');

		this.Permission = Permission.Unknown;
		this.Region = ServerRegion.USW;
		this.CustomRegion = "";

		string[] visibleInfo = splitId[0].Split(':');

		this.WorldId = visibleInfo[0];

		if (visibleInfo.Length != 2) {
			this.Permission = Permission.Unknown;
			return;
		}

			this.Permission = Permission.Public;
			this.InstanceName = visibleInfo[1];

			var containsCanRequestInvite = false;


			for (var i = 1; i < splitId.Length; i++) {
				if (splitId[i] == "canRequestInvite") {
					
					containsCanRequestInvite = true;
					continue;
				}

				string pKey, pValue;

				{
					var splitParam = splitId[i].Split('(');

					if (2 < splitParam.Length) continue;

					pKey = splitParam[0];

					if (splitParam.Length == 2) {
						pValue = splitParam[1].Substring(0, splitParam[1].Length-1);
						if (pValue == "") pValue = null;
					} else {
						pValue = null;
					}
				}

				switch (pKey) {
					case "region":
						switch (pValue) {
						case "us":
							this.Region = ServerRegion.USW;
							break;
						
							case "use":
								this.Region = ServerRegion.USE;
								break;

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
						
						this.Nonce = pValue;
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

		public Instance(string id, string worldName) {
			
			parseId(id);
			this.WorldName = worldName;
		}
	}
}

