using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

partial class MainForm : Form {
	PictureBox	logo;
	Button		launchVrc,
				detail,
				next,
				prev;
	Label		datetime,
				instance,
				permission;
	List<Visit>	sortedHistory;

	IContainer			components;
	ContextMenuStrip	instanceIdContextMenu;
	ToolStripMenuItem	copyLaunchInstanceLink,
						copyInstanceLink,
						saveLaunchInstanceLink;

	int index = 0;
	bool killVRC;

	void copyInstanceLinkClick(object sender, EventArgs e) {
		Clipboard.SetText(
			VRChat.GetInstanceLink(
				sortedHistory[index].Instance
			)
		);
	}

	void saveInstanceToShortcutGUI(Instance i) {
		var sfd = new SaveFileDialog();

		var filename = i.WorldId;
		var idWithoutWorldId = i.IdWithoutWorldId;

		if (idWithoutWorldId != "") {
			filename += "-";
			filename += idWithoutWorldId;
		}

		filename += ".lnk";

		sfd.FileName = filename;

		sfd.Filter = "Link (*.lnk)|*.lnk|All files (*.*)|*.*";
		sfd.Title = "Save Instance";

		if (sfd.ShowDialog() != DialogResult.OK) return;

		VRChat.SaveInstanceToShortcut(i, sfd.FileName);
	}

	void saveLaunchInstanceLinkClick(object sender, EventArgs e) {
		saveInstanceToShortcutGUI(sortedHistory[index].Instance);
	}

	void copyLaunchInstanceLinkClick(object sender, EventArgs e) {
		Clipboard.SetText(
			VRChat.GetLaunchInstanceLink(
				sortedHistory[index].Instance
			)
		);
	}

	void detailButtonClick(object sender, EventArgs e) {
		Process.Start(
			VRChat.GetInstanceLink(
				sortedHistory[index].Instance
			)
		);
	}

	void launchVrcButtonClick(object sender, EventArgs e) {
		VRChat.Launch(
			sortedHistory[index].Instance,
			killVRC
		);

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

