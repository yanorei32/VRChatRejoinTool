using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

partial class EditInstanceForm : RejoinToolForm {
	// UI Elements
	ComboBox	permission;

	TextBox		worldId,
				instanceName,
				ownerId,
				nonce;

	Label		worldIdLabel,
				permissionLabel,
				instanceNameLabel,
				ownerIdLabel,
				nonceLabel,
				instanceId,
				instanceIdLabel;

	Button		launchVrc,
				detail;

	// ContextMenu
	IContainer			components;
	ContextMenuStrip	instanceIdContextMenu;
	ToolStripMenuItem	copyLaunchInstanceLink,
						copyInstanceLink,
						saveLaunchInstanceLink;
	
	// Other instance variables
	Instance	instance;
	bool		killVRC;

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		this.permission.SelectedItem = this.instance.Permission;
	}

	void updateInstanceId() {
		this.instanceId.Text = instance.Id;
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
		|*| Instance Name
		\*/
		instanceNameLabel.Text = "Instance Name";

		if (instance.Permission != Permission.Unknown) {
			if (instance.InstanceName == null) {
				instanceNameLabel.Text += " (required)";
				instanceNameLabel.ForeColor = Color.Red;
			} else if (!instance.IsMaybeValidInstanceName()) {
				instanceNameLabel.Text += " (maybe-invalid)";
				instanceNameLabel.ForeColor = Color.Red;
			} else if (!instance.IsSafeInstanceName()) {
				instanceNameLabel.Text += " (maybe-valid)";
				instanceNameLabel.ForeColor = Color.Orange;
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
					nonceLabel.Text += " (maybe-invalid)";
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
	}

	void updatePermission() {
		nonce.Enabled
			= instanceName.Enabled
			= ownerId.Enabled
			= instance.Permission != Permission.Unknown;
	}

	void permissionChanged(object sender, EventArgs e) {
		instance.Permission = (Permission) permission.SelectedItem;

		updatePermission();
		updateTextBox();
		updateInstanceId();
	}

	void textBoxChanged(object sender, EventArgs e) {
		instance.WorldId = worldId.Text;
		instance.InstanceName = instanceName.Text;
		instance.OwnerId = ownerId.Text == "" ? null : ownerId.Text;
		instance.Nonce = nonce.Text == "" ? null : nonce.Text;

		updateTextBox();
		updateInstanceId();
	}

	void launchVrcButtonClick(object sender, EventArgs e) {
		VRChat.Launch(instance, this.killVRC);
	}

	void detailButtonClick(object sender, EventArgs e) {
		showDetail(instance);
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

	public EditInstanceForm(Instance instance, bool killVRC) {
		this.instance = instance;
		this.killVRC = killVRC;

		initializeComponent();

		updatePermission();
		updateTextBox();
		updateInstanceId();
	}
}

