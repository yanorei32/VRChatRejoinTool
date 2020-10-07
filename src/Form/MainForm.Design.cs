using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

partial class MainForm : RejoinToolForm {
	void initializeComponent() {
		const int
			imgW	= 320,
			imgH	= 84,
			margin	= 12,
			padding	= 6;

		int curW = 0, curH = 0;
		Assembly execAsm = Assembly.GetExecutingAssembly();

		/*\
		|*| Contextmenu Initialization
		\*/
		this.components				= new Container();
		this.instanceIdContextMenu	= new ContextMenuStrip(components);
		this.copyLaunchInstanceLink	= new ToolStripMenuItem();
		this.copyInstanceLink		= new ToolStripMenuItem();
		this.saveLaunchInstanceLink	= new ToolStripMenuItem();
		this.saveInstanceLink		= new ToolStripMenuItem();
		this.editInstance			= new ToolStripMenuItem();

		this.instanceIdContextMenu.SuspendLayout();

		this.copyInstanceLink.Text			= "Copy Instance (https://) Link (&C)";
		this.copyInstanceLink.Click			+= new EventHandler(copyInstanceLinkClick);
		this.copyInstanceLink.ShortcutKeys	= Keys.Control | Keys.C;

		this.copyLaunchInstanceLink.Text 			= "Copy Instance (vrchat://) Link (&V)";
		this.copyLaunchInstanceLink.Click			+= new EventHandler(copyLaunchInstanceLinkClick);
		this.copyLaunchInstanceLink.ShortcutKeys	= Keys.Control | Keys.Shift | Keys.C;

		this.saveLaunchInstanceLink.Text			= "Save Instance (vrchat://) Shortcut (&S)";
		this.saveLaunchInstanceLink.Click			+= new EventHandler(saveLaunchInstanceLinkClick);
		this.saveLaunchInstanceLink.ShortcutKeys	= Keys.Control | Keys.S;

		this.saveInstanceLink.Text			= "Save Instance (https://) Shortcut (&S)";
		this.saveInstanceLink.Click			+= new EventHandler(saveInstanceLinkClick);
		this.saveInstanceLink.ShortcutKeys	= Keys.Control | Keys.Shift | Keys.S;

		this.editInstance.Text			= "Edit Instance (&E)";
		this.editInstance.Click			+= new EventHandler(editInstanceClick);
		this.editInstance.ShortcutKeys	= Keys.Control | Keys.E;

		this.instanceIdContextMenu.Items.Add(this.copyLaunchInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.copyInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.saveLaunchInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.saveInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.editInstance);

		this.instanceIdContextMenu.ResumeLayout(false);

		/*\
		|*| UI Initialization
		\*/
		this.logo		= new PictureBox();
		this.prev		= new Button();
		this.next		= new Button();
		this.launchVrc	= new Button();
		this.detail		= new Button();
		this.userDetail	= new Button();
		this.datetime	= new Label();
		this.instance	= new Label();
		this.permission	= new Label();

		this.SuspendLayout();
		curH = padding;
		curW = margin;

		/*\
		|*| Logo column
		\*/
		this.logo.Location			= new Point(curH, curW);
		this.logo.Size				= new Size(imgW, imgH);
		this.logo.BackgroundImage	= new Bitmap(
			execAsm.GetManifestResourceStream("logo")
		);

		curH += this.logo.Size.Height;
		curH += padding;

		/*\
		|*| Prev/Next button column
		\*/
		this.prev.Text			= "< Newer (&N)";
		this.prev.Size			= new Size(75, 23);
		this.prev.Location		= new Point(curW, curH);
		this.prev.Click			+= new EventHandler(prevButtonClick);
		this.prev.UseMnemonic	= true;


		curW += this.prev.Size.Width;
		curW += padding;

		this.next.Text			= "(&O) Older >";
		this.next.Size			= new Size(75, 23);
		this.next.Location		= new Point(curW, curH);
		this.next.Click			+= new EventHandler(nextButtonClick);
		this.next.UseMnemonic	= true;

		curW = margin;
		curH += this.next.Size.Height;
		curH += padding;

		/*\
		|*| Joined date time column
		\*/
		this.datetime.Text		= "Date: 0000.00.00 00:00:00";
		this.datetime.AutoSize	= false;
		this.datetime.Location	= new Point(curW, curH);
		this.datetime.Size		= new Size(imgW, 22);
		this.datetime.Font		= new Font("Consolas", 16F);

		curH += this.datetime.Size.Height;
		curH += padding;

		/*\
		|*| Permission column
		\*/
		this.permission.Text		= "Permission: XXX";
		this.permission.AutoSize	= false;
		this.permission.Location	= new Point(curW, curH);
		this.permission.Size		= new Size(imgW, 20);
		this.permission.Font		= new Font("Consolas", 14F);

		curH += this.permission.Size.Height;
		curH += padding;

		/*\
		|*| Instance column
		\*/
		this.instance.Text		= "Instance: wrld_xxx";
		this.instance.AutoSize	= false;
		this.instance.Location	= new Point(curW, curH);
		this.instance.Size		= new Size(imgW, 75);
		this.instance.Font		= new Font("Consolas", 9F);

		curH += this.instance.Size.Height;
		curH += padding;

		/*\
		|*| Launch button column
		\*/
		this.launchVrc.Text			= "Launch (&L)";
		this.launchVrc.Location		= new Point(curW, curH);
		this.launchVrc.Size			= new Size(75, 23);
		this.launchVrc.Click		+= new EventHandler(launchVrcButtonClick);
		this.launchVrc.UseMnemonic	= true;

		curW += this.launchVrc.Size.Width;
		curW += padding;

		this.detail.Text		= "Detail (&D)";
		this.detail.Location	= new Point(curW, curH);
		this.detail.Size		= new Size(75, 23);
		this.detail.Click		+= new EventHandler(detailButtonClick);
		this.detail.UseMnemonic	= true;

		curW += this.detail.Size.Width;
		curW += padding;

		this.userDetail.Text		= "User (&U)";
		this.userDetail.Location	= new Point(curW, curH);
		this.userDetail.Size		= new Size(75, 23);
		this.userDetail.Click		+= new EventHandler(userDetailButtonClick);
		this.userDetail.UseMnemonic	= true;

		curW = margin;
		curH += this.launchVrc.Size.Height;
		curH += padding;

		/*\
		|*| Form
		\*/
		this.Text				= "VRChat RejoinTool";

#if NETCOREAPP
		this.ClientSize			= new Size(imgW + (margin * 2), curH);
#else
		// net framework special fix
		this.ClientSize			= new Size(imgW + (margin * 2) - 10, curH - 10);
#endif

		this.MinimumSize		= this.Size;
		this.MaximumSize		= this.Size;
		this.FormBorderStyle	= FormBorderStyle.FixedSingle;
		this.Icon				= new Icon(execAsm.GetManifestResourceStream("icon"));
		this.ContextMenuStrip	= instanceIdContextMenu;
		this.Controls.Add(this.logo);
		this.Controls.Add(this.launchVrc);
		this.Controls.Add(this.detail);
		this.Controls.Add(this.userDetail);
		this.Controls.Add(this.prev);
		this.Controls.Add(this.next);
		this.Controls.Add(this.datetime);
		this.Controls.Add(this.instance);
		this.Controls.Add(this.permission);
		this.ResumeLayout(false);
	}
}

