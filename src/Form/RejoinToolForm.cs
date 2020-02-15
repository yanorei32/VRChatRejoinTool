using System.Diagnostics;
using System.Windows.Forms;

class RejoinToolForm : Form {
	protected void saveInstanceToShortcutGUI(Instance i, bool httpLink = false) {
		var sfd = new SaveFileDialog();

		var filename = i.WorldId;
		var idWithoutWorldId = i.IdWithoutWorldId;

		if (httpLink)
			filename = "web-" + filename;

		if (idWithoutWorldId != "") {
			filename += "-";
			filename += idWithoutWorldId;
		}

		filename += ".lnk";

		sfd.FileName = filename;

		sfd.Filter = "Link (*.lnk)|*.lnk|All files (*.*)|*.*";
		sfd.Title = "Save Instance";

		if (sfd.ShowDialog() != DialogResult.OK) return;

		VRChat.SaveInstanceToShortcut(i, sfd.FileName, httpLink);
	}

	protected void copyLaunchInstanceLinkToClipboard(Instance i) {
		Clipboard.SetText(VRChat.GetLaunchInstanceLink(i));
	}

	protected void copyInstanceLinkToClipboard(Instance i) {
		Clipboard.SetText(VRChat.GetInstanceLink(i));
	}

	protected void showDetail(Instance i) {
		Process.Start(VRChat.GetInstanceLink(i));
	}

	protected void showUserDetail(Instance i) {
		Process.Start(VRChat.GetUserIdLink(i));
	}
}

