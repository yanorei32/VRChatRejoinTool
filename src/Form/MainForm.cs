using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

partial class MainForm : RejoinToolForm {
	// UI Elements
	PictureBox	logo;

	Button	launchVrc,
			inviteMe,
			detail,
			next,
			prev,
			userDetail;

	Label	datetime,
			instance,
			worldname,
			permission;

	// ContextMenu
	IContainer			components;
	ContextMenuStrip	instanceIdContextMenu;
	ToolStripMenuItem	copyLaunchInstanceLink,
						copyInstanceLink,
						saveInstanceLink,
						saveLaunchInstanceLink,
						editInstance;

	// Other instance variables
	List<Visit>	sortedHistory;
	int index = 0;
	bool killVRC;
	string vrcInviteMePath;

	void editInstanceClick(object sender, EventArgs e) {
		Instance i = sortedHistory[index].Instance.ShallowCopy();
		i.WorldName = null;
		(new EditInstanceForm(i, killVRC, vrcInviteMePath)).Show();
	}

	void copyInstanceLinkClick(object sender, EventArgs e) {
		copyInstanceLinkToClipboard(sortedHistory[index].Instance);
	}

	void saveInstanceLinkClick(object sender, EventArgs e) {
		saveInstanceToShortcutGUI(sortedHistory[index].Instance, true);
	}

	void saveLaunchInstanceLinkClick(object sender, EventArgs e) {
		saveInstanceToShortcutGUI(sortedHistory[index].Instance);
	}

	void copyLaunchInstanceLinkClick(object sender, EventArgs e) {
		copyLaunchInstanceLinkToClipboard(sortedHistory[index].Instance);
	}

	void detailButtonClick(object sender, EventArgs e) {
		showDetail(sortedHistory[index].Instance);
	}

	void userDetailButtonClick(object sender, EventArgs e) {
		showUserDetail(sortedHistory[index].Instance);
	}

	void inviteMeButtonClick(object sender, EventArgs e) {
		if (VRChat.InviteMe(sortedHistory[index].Instance, vrcInviteMePath) == 0) {
			this.Close();
		} else {
			MessageBox.Show("Check your vrc-invite-me.exe setting");
		}
	}

	void launchVrcButtonClick(object sender, EventArgs e) {
		VRChat.Launch(sortedHistory[index].Instance, killVRC);

		this.Close();
	}

	void prevButtonClick(object sender, EventArgs e) {
		index --;
		update();
	}

	void nextButtonClick(object sender, EventArgs e) {
		index ++;
		update();
	}

	void update() {
		Visit v = sortedHistory[index];

		this.worldname.Text = v.Instance.WorldName == null ? "(!) Failed to Joining" : v.Instance.WorldName;
		this.instance.Text = v.Instance.Id;
		this.datetime.Text = " " + v.DateTime.ToString("yyyy/MM/dd HH:mm:ss");
		this.permission.Text = " " + Enum.GetName(
			typeof(Permission),
			v.Instance.Permission
		);
		this.prev.Enabled = 0 < index;
		this.next.Enabled = index < sortedHistory.Count - 1;
		this.userDetail.Enabled = v.Instance.OwnerId != null;
	}

	public MainForm(List<Visit> sortedHistory, bool killVRC, string vrcInviteMePath) {
		this.vrcInviteMePath = vrcInviteMePath;
		this.killVRC = killVRC;
		this.sortedHistory = sortedHistory;
		initializeComponent();
		update();
	}
}

