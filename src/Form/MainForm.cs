using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

partial class MainForm : RejoinToolForm {
	// UI Elements
	PictureBox	logo;

	Button	launchVrc,
			detail,
			next,
			prev;

	Label	datetime,
			instance,
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

	void editInstanceClick(object sender, EventArgs e) {
		Instance i = sortedHistory[index].Instance.ShallowCopy();
		(new EditInstanceForm(i, killVRC)).Show();
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

		this.instance.Text = "Instance:\n" + v.Instance.Id;
		this.datetime.Text = "Date: " + v.DateTime.ToString("yyyy/MM/dd HH:mm:ss");
		this.permission.Text = "Permission: " + Enum.GetName(
			typeof(Permission),
			v.Instance.Permission
		);
		this.prev.Enabled = 0 < index;
		this.next.Enabled = index < sortedHistory.Count - 1;
	}

	public MainForm(List<Visit> sortedHistory, bool killVRC) {
		this.killVRC = killVRC;
		this.sortedHistory = sortedHistory;
		initializeComponent();
		update();
	}
}

