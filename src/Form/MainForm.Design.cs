using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

partial class MainForm : Form {
	void initializeComponent() {
		const int
			imgW	= 320,
			imgH	= 84,
			margin	= 13,
			padding	= 6;

		int curW = 0, curH = 0;
		Assembly execAsm = Assembly.GetExecutingAssembly();

		this.components				= new Container();
		this.instanceIdContextMenu	= new ContextMenuStrip(components);
		this.copyLaunchInstanceLink	= new ToolStripMenuItem();
		this.copyInstanceLink		= new ToolStripMenuItem();
		this.saveLaunchInstanceLink	= new ToolStripMenuItem();

		this.logo		= new PictureBox();
		this.prev		= new Button();
		this.next		= new Button();
		this.launchVrc	= new Button();
		this.detail		= new Button();
		this.datetime	= new Label();
		this.instance	= new Label();
		this.permission	= new Label();

		this.instanceIdContextMenu.SuspendLayout();

		this.copyInstanceLink.Text			= "Copy Instance (https://) Link (&C)";
		this.copyInstanceLink.Click			+= new EventHandler(copyInstanceLinkClick);
		this.copyInstanceLink.ShortcutKeys	= Keys.Control | Keys.C;

		this.copyLaunchInstanceLink.Text 			= "Copy Instance (vrchat://) Link (&V)";
		this.copyLaunchInstanceLink.Click			+= new EventHandler(copyLaunchInstanceLinkClick);
		this.copyLaunchInstanceLink.ShortcutKeys	= Keys.Control | Keys.Shift | Keys.C;

		this.saveLaunchInstanceLink.Text			= "Save Instance Shortcut (&S)";
		this.saveLaunchInstanceLink.Click			+= new EventHandler(saveLaunchInstanceLinkClick);
		this.saveLaunchInstanceLink.ShortcutKeys	= Keys.Control | Keys.S;

		this.instanceIdContextMenu.Items.Add(this.copyLaunchInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.copyInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.saveLaunchInstanceLink);

		this.instanceIdContextMenu.ResumeLayout(false);

		this.SuspendLayout();
		curH = curW = margin;

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
		this.prev.Text		= "< Newer";
		this.prev.Size		= new Size(75, 23);
		this.prev.Location	= new Point(curW, curH);
		this.prev.Click		+= new EventHandler(prevButtonClick);

		curW += this.prev.Size.Width;
		curW += padding;

		this.next.Text		= "Older >";
		this.next.Size		= new Size(75, 23);
		this.next.Location	= new Point(curW, curH);
		this.next.Click		+= new EventHandler(nextButtonClick);

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
		this.datetime.Font		= new Font("Conolas", 16F);

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
		this.launchVrc.Text		= "Launch";
		this.launchVrc.Location	= new Point(curW, curH);
		this.launchVrc.Size		= new Size(75, 23);
		this.launchVrc.Click	+= new EventHandler(launchVrcButtonClick);

		curW += this.launchVrc.Size.Width;
		curW += padding;

		this.detail.Text		= "Detail";
		this.detail.Location	= new Point(curW, curH);
		this.detail.Size		= new Size(75, 23);
		this.detail.Click		+= new EventHandler(detailButtonClick);

		curW = margin;
		curH += this.launchVrc.Size.Height;
		// curH += margin;

		/*\
		|*| Form
		\*/
		this.Text				= "VRChat RejoinTool";
		this.ClientSize			= new Size(imgW + (margin * 2), curH);
		this.MinimumSize		= this.Size;
		this.MaximumSize		= this.Size;
		this.FormBorderStyle	= FormBorderStyle.FixedSingle;
		this.Icon				= new Icon(execAsm.GetManifestResourceStream("icon"));
		this.ContextMenuStrip	= instanceIdContextMenu;
		this.Controls.Add(this.logo);
		this.Controls.Add(this.launchVrc);
		this.Controls.Add(this.detail);
		this.Controls.Add(this.prev);
		this.Controls.Add(this.next);
		this.Controls.Add(this.datetime);
		this.Controls.Add(this.instance);
		this.Controls.Add(this.permission);
		this.ResumeLayout(false);
	}
}

