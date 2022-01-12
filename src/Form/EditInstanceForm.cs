using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using VRChatRejoinTool.Utility;

namespace VRChatRejoinTool.Form {
	partial class EditInstanceForm : System.Windows.Forms.Form {
		// UI Elements
		ComboBox	permission,
			region;

		TextBox		worldId,
			customRegion,
			instanceName,
			ownerId,
			nonce;

		

		Label		worldIdLabel,
			regionLabel,
			permissionLabel,
			instanceNameLabel,
			ownerIdLabel,
			nonceLabel,
			
			instanceId,
			instanceIdLabel;

		Button		
			launchVrc,
			inviteMe,
			detail,
			userDetail;

		// ContextMenu
		IContainer			components;
		ContextMenuStrip	instanceIdContextMenu;
		ToolStripMenuItem	copyLaunchInstanceLink,
			copyInstanceLink,
			saveLaunchInstanceLink,
			saveInstanceLink;
	
		// Other instance variables
		Instance	instance;
		bool		killVRC;
		string		vrcInviteMePath;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.permission.SelectedItem = this.instance.Permission;
			this.region.SelectedItem = this.instance.Region;
		}

		void updateInstanceId() {
			this.instanceId.Text = instance.Id;
		}

	void updateTextBox() {
		/*\
		|*| World ID
		\*/
		worldIdLabel.Text = "World ID";
		worldIdLabel.ForeColor = Color.Black;

			if (!instance.IsValidWorldId()) {
				worldIdLabel.Text += " (invalid)";
				worldIdLabel.ForeColor = Color.Red;
			}

			/*\
			|*| Region
			\*/
			regionLabel.Text = "Region";
			regionLabel.ForeColor = Color.Black;

			if (customRegion.Enabled) {
				if (!instance.IsValidCustomRegionName()) {
					regionLabel.Text += " (invalid)";
					regionLabel.ForeColor = Color.Red;
				}
			}

			/*\
			|*| Instance Name
			\*/
			instanceNameLabel.Text = "Instance Name";
			instanceNameLabel.ForeColor = Color.Black;

			if (instanceName.Enabled) {
				if (instance.InstanceName == null) {
					instanceNameLabel.Text += " (required)";
					instanceNameLabel.ForeColor = Color.Red;
				} else if (!instance.IsValidInstanceName()) {
					instanceNameLabel.Text += " (invalid)";
					instanceNameLabel.ForeColor = Color.Red;
				}
			}

			/*\
			|*| Owner ID
			\*/
			ownerIdLabel.Text = "Owner ID";
			ownerIdLabel.ForeColor = Color.Black;

			if (ownerId.Enabled) {
				if (instance.OwnerId == null) {
					ownerIdLabel.Text += " (required)";
					ownerIdLabel.ForeColor = Color.Red;
				} else if (!instance.IsValidUserId()) {
					ownerIdLabel.Text += " (invalid)";
					ownerIdLabel.ForeColor = Color.Red;
				}
			}

			/*\
			|*| Nonce
			\*/
			nonceLabel.Text = "Nonce";
			nonceLabel.ForeColor = Color.Black;

			if (nonce.Enabled) {
				if (instance.Nonce == null) {
				nonceLabel.Text += " (required)";
				nonceLabel.ForeColor = Color.Red;
			} else if (!instance.IsValidNonceValue()) {
					nonceLabel.Text += " (invalid)";
					nonceLabel.ForeColor = Color.Red;
				}
			}

			userDetail.Enabled = ownerId.Enabled && instance.OwnerId != null;
		}

		void updateRegion() {
			customRegion.Enabled = region.Enabled && (instance.Region == ServerRegion.Custom);
		}

	void updatePermission() {
		nonce.Enabled
			= instanceName.Enabled
			= ownerId.Enabled
			= region.Enabled
			= instance.Permission != Permission.Unknown;

		ownerId.Enabled &= instance.Permission != Permission.Public;
		nonce.Enabled &= instance.Permission != Permission.Public;
	}

		void permissionChanged(object sender, EventArgs e) {
			instance.Permission = (Permission) permission.SelectedItem;

			updatePermission();
			updateRegion();
			updateTextBox();
			updateInstanceId();
		}

		void regionChanged(object sender, EventArgs e) {
			instance.Region = (ServerRegion) region.SelectedItem;

			updateRegion();
			updateTextBox();
			updateInstanceId();
		}

		void textBoxChanged(object sender, EventArgs e) {
			instance.WorldId = worldId.Text;
			instance.InstanceName = instanceName.Text;
			instance.CustomRegion = customRegion.Text;
			instance.OwnerId = ownerId.Text == "" ? null : ownerId.Text;
			instance.Nonce = nonce.Text == "" ? null : nonce.Text;

			updateTextBox();
			updateInstanceId();
		}

		void nonceLabelDoubleClick(object sender, EventArgs e) {
			if (!nonce.Enabled)
				return;

			nonce.Text = Guid.NewGuid().ToString();
		}

		void launchVrcButtonClick(object sender, EventArgs e) {
			VRChat.Launch(instance, this.killVRC);
		}

		void inviteMeButtonClick(object sender, EventArgs e) {
			if (VRChat.InviteMe(instance, vrcInviteMePath) == 0) {
				this.Close();
			} else {
				MessageBox.Show("Check your vrc-invite-me.exe setting");
			}
		}

		void detailButtonClick(object sender, EventArgs e) {
			ShellUtility.ShowDetail(instance);
		}

		void userDetailButtonClick(object sender, EventArgs e) {
			ShellUtility.ShowUserDetail(instance);
		}

		void copyLaunchInstanceLinkClick(object sender, EventArgs e) {
			ClipboardUtility.CopyLaunchInstanceLinkToClipboard(instance);
		}

		void windowKeyDown(object sender, KeyEventArgs e) {
			if ((e.KeyData & e.KeyCode) == Keys.Escape)
				this.Close();
		}

		void openContextMenu(object sender, EventArgs e) {
			// dirty Ctrl+C override avoidance (1/2)
			if (ActiveControl.GetType().Name == "TextBox") {
				var t = (TextBox) ActiveControl;
				t.DeselectAll();
			}
		}

		void copyInstanceLinkClick(object sender, EventArgs e) {
			// dirty Ctrl+C override avoidance (2/2)
			if (ActiveControl.GetType().Name == "TextBox") {
				var t = (TextBox) ActiveControl;

				if (t.SelectionLength != 0) {
					Clipboard.SetText(t.SelectedText);
					return;
				}
			}

			ClipboardUtility.CopyInstanceLinkToClipboard(instance);
		}

		void saveLaunchInstanceLinkClick(object sender, EventArgs e) {
			SaveInstanceUtility.SaveInstanceToShortcutGui(instance, false);
		}

		void saveInstanceLinkClick(object sender, EventArgs e) {
			SaveInstanceUtility.SaveInstanceToShortcutGui(instance, true);
		}

		public EditInstanceForm(Instance instance, bool killVRC, string vrcInviteMePath) {
			this.instance = instance;
			this.vrcInviteMePath = vrcInviteMePath;
			this.killVRC = killVRC;

			initializeComponent();
			
			updatePermission();
			updateRegion();
			updateTextBox();
			updateInstanceId();
		}
	}
}

