using System.Windows.Forms;

namespace VRChatRejoinTool.Form {
	class RejoinToolForm : System.Windows.Forms.Form {
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
	}
}

