using System;
using System.Collections.Generic;
using System.Windows.Forms;

partial class MainForm : Form {
	PictureBox logo;
	Button launchVrc, showInVrcw, next, prev;
	Label datetime, instance, permission;
	List<Visit> sortedHistory;
	bool killVRC;
	int index = 0;

	void prevButtonClick(object sender, EventArgs e) {
		index--;
		update();
	}

	void nextButtonClick(object sender, EventArgs e) {
		index++;
		update();
	}

	void launchVrcButtonClick(object sender, EventArgs e) {
		VRChat.Launch(sortedHistory[index].Instance.RawId, killVRC);
		this.Close();
	}

	void showInVrcwButtonClick(object sender, EventArgs e) {
		VRChat.OpenVRCW(sortedHistory[index].Instance.WorldId);
	}

	void update() {
		Visit v = sortedHistory[index];

		this.instance.Text = "Instance:\n" + v.Instance.RawId;
		this.datetime.Text = "Date: " + v.DateTime.ToString();
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

