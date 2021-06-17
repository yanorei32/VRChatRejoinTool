using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

partial class EditInstanceForm : RejoinToolForm {
	// UI Elements
	ComboBox	permission,
				region;

	TextBox		worldId,
				customRegion,
				instanceName,
				ownerId,
				nonce;

	ListBox		argumentOrder;

	Label		worldIdLabel,
				regionLabel,
				permissionLabel,
				instanceNameLabel,
				ownerIdLabel,
				nonceLabel,
				argumentOrderLabel,
				instanceId,
				instanceIdLabel;

	Button		orderUp,
				orderDown,
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

	void updateListBox() {
		argumentOrder.BeginUpdate();
		argumentOrder.Items.Clear();

		for (int i = 0; i < instance.ArgumentOrder.Length; i++) {
			argumentOrder.Items.Add(
				Enum.GetName(typeof(InstanceArgument), instance.ArgumentOrder[i])
			);
		}

		argumentOrder.EndUpdate();

		argumentOrderLabel.Text = "Argument Order";
		if (!instance.IsValidArgumentOrder()) {
			argumentOrderLabel.Text += " (mustbe P/CRI/R/N)";
			argumentOrderLabel.ForeColor = Color.Red;
		} else {
			argumentOrderLabel.ForeColor = Color.Black;
		}

		updateOrderButton();
	}

	void updateTextBox() {
		/*\
		|*| World ID
		\*/
		worldIdLabel.Text = "World ID";

		if (!instance.IsValidWorldId()) {
			worldIdLabel.Text += " (invalid)";
			worldIdLabel.ForeColor = Color.Red;
		} else {
			worldIdLabel.ForeColor = Color.Black;
		}

		/*\
		|*| Region
		\*/
		regionLabel.Text = "Region";

		if (instance.Permission != Permission.Unknown) {
			if (
				instance.Region == ServerRegion.Custom
				&&
				!instance.IsValidCustomRegionName()
			) {
				regionLabel.Text += " (invalid)";
				regionLabel.ForeColor = Color.Red;
			} else {
				regionLabel.ForeColor = Color.Black;
			}
		} else {
			regionLabel.ForeColor = Color.Black;
		}

		/*\
		|*| Instance Name
		\*/
		instanceNameLabel.Text = "Instance Name";

		if (instance.Permission != Permission.Unknown) {
			if (instance.InstanceName == null) {
				instanceNameLabel.Text += " (required)";
				instanceNameLabel.ForeColor = Color.Red;
			} else if (!instance.IsValidInstanceName()) {
				instanceNameLabel.Text += " (invalid)";
				instanceNameLabel.ForeColor = Color.Red;
			} else {
				instanceNameLabel.ForeColor = Color.Black;
			}
		} else {
			instanceNameLabel.ForeColor = Color.Black;
		}

		/*\
		|*| Owner ID
		\*/
		ownerIdLabel.Text = "Owner ID";

		if (instance.Permission != Permission.Unknown) {
			if (instance.Permission != Permission.Public && instance.Permission != Permission.PublicWithIdentifier) {
				if (instance.OwnerId == null) {
					ownerIdLabel.Text += " (required)";
					ownerIdLabel.ForeColor = Color.Red;
				} else if (!instance.IsValidUserId()) {
					ownerIdLabel.Text += " (invalid)";
					ownerIdLabel.ForeColor = Color.Red;
				} else {
					ownerIdLabel.ForeColor = Color.Black;
				}
			} else {
				ownerIdLabel.Text += " (invalid when set)";
				ownerIdLabel.ForeColor = instance.OwnerId == null ? Color.Black : Color.Red;
			}
		} else {
			ownerIdLabel.ForeColor = Color.Black;
		}

		/*\
		|*| Nonce
		\*/
		nonceLabel.Text = "Nonce";

		if (instance.Permission != Permission.Unknown) {
			if (instance.Permission != Permission.Public && instance.Permission != Permission.PublicWithIdentifier) {
				if (instance.Nonce == null) {
					nonceLabel.Text += " (required)";
					nonceLabel.ForeColor = Color.Red;
				} else if (!instance.IsValidNonceValue()) {
					nonceLabel.Text += " (invalid)";
					nonceLabel.ForeColor = Color.Red;
				} else {
					nonceLabel.ForeColor = Color.Black;
				}
			} else {
				nonceLabel.Text += " (invalid when set)";
				nonceLabel.ForeColor = instance.Nonce == null ? Color.Black : Color.Red;
			}
		} else {
			nonceLabel.ForeColor = Color.Black;
		}

		userDetail.Enabled = instance.OwnerId != null;
	}

	void updateRegion() {
		customRegion.Enabled = instance.Region == ServerRegion.Custom;
	}

	void updatePermission() {
		nonce.Enabled
			= instanceName.Enabled
			= ownerId.Enabled
			= region.Enabled
			= customRegion.Enabled
			= instance.Permission != Permission.Unknown;

		permissionLabel.Text = "Permission";

		if (!instance.IsValidPermission()) {
			permissionLabel.Text += " (obsolete)";
			permissionLabel.ForeColor = Color.Red;
		} else {
			permissionLabel.ForeColor = Color.Black;
		}

		updateRegion();
	}

	void permissionChanged(object sender, EventArgs e) {
		instance.Permission = (Permission) permission.SelectedItem;

		updatePermission();
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
		if (instance.Permission == Permission.Unknown)
			return;

		nonce.Text = Guid.NewGuid().ToString();
	}

	void updateOrderButton() {
		orderUp.Enabled = 0 < argumentOrder.SelectedIndex;
		orderDown.Enabled = argumentOrder.SelectedIndex != -1 && argumentOrder.SelectedIndex < instance.ArgumentOrder.Length - 1;
	}

	void argumentOrderSelectedIndexChanged(object sender, EventArgs e) {
		updateOrderButton();
	}

	void orderDownButtonClick(object sender, EventArgs e) {
		int i = argumentOrder.SelectedIndex;
		InstanceArgument temp = instance.ArgumentOrder[i];
		instance.ArgumentOrder[i] = instance.ArgumentOrder[i+1];
		instance.ArgumentOrder[i+1] = temp;
		updateListBox();
		argumentOrder.SelectedIndex = i+1;
		updateOrderButton();
		updateInstanceId();
	}

	void orderUpButtonClick(object sender, EventArgs e) {
		int i = argumentOrder.SelectedIndex;
		InstanceArgument temp = instance.ArgumentOrder[i];
		instance.ArgumentOrder[i] = instance.ArgumentOrder[i-1];
		instance.ArgumentOrder[i-1] = temp;
		updateListBox();
		argumentOrder.SelectedIndex = i-1;
		updateOrderButton();
		updateInstanceId();
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
		showDetail(instance);
	}

	void userDetailButtonClick(object sender, EventArgs e) {
		showUserDetail(instance);
	}

	void copyLaunchInstanceLinkClick(object sender, EventArgs e) {
		copyLaunchInstanceLinkToClipboard(instance);
	}

	void windowKeyDown(object sender, KeyEventArgs e) {
		if ((e.KeyData & e.KeyCode) == Keys.Escape)
			this.Close();
	}

	void openContextMenu(object sender, EventArgs e) {
		// dirty Ctrl+C override avoidance (1/2)
		if (ActiveControl.GetType().Name == "TextBox") {
			TextBox t = (TextBox) ActiveControl;
			t.DeselectAll();
		}
	}

	void copyInstanceLinkClick(object sender, EventArgs e) {
		// dirty Ctrl+C override avoidance (2/2)
		if (ActiveControl.GetType().Name == "TextBox") {
			TextBox t = (TextBox) ActiveControl;

			if (t.SelectionLength != 0) {
				Clipboard.SetText(t.SelectedText);
				return;
			}
		}

		copyInstanceLinkToClipboard(instance);
	}

	void saveLaunchInstanceLinkClick(object sender, EventArgs e) {
		saveInstanceToShortcutGUI(instance);
	}

	void saveInstanceLinkClick(object sender, EventArgs e) {
		saveInstanceToShortcutGUI(instance, true);
	}

	public EditInstanceForm(Instance instance, bool killVRC, string vrcInviteMePath) {
		this.instance = instance;
		this.vrcInviteMePath = vrcInviteMePath;
		this.killVRC = killVRC;

		initializeComponent();

		updateListBox();
		updatePermission();
		updateRegion();
		updateTextBox();
		updateInstanceId();
	}
}

