using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

partial class EditInstanceForm : RejoinToolForm {
	void initializeComponent() {
		const int
			textBoxW = 320,
			margin	= 13,
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

		this.instanceIdContextMenu.Opened	+= new EventHandler(openContextMenu);
		this.instanceIdContextMenu.Items.Add(this.copyLaunchInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.copyInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.saveLaunchInstanceLink);
		this.instanceIdContextMenu.Items.Add(this.saveInstanceLink);

		this.instanceIdContextMenu.ResumeLayout(false);

		/*\
		|*| UI Initialization
		\*/
		this.worldIdLabel		= new Label();
		this.worldId			= new TextBox();
		this.permissionLabel	= new Label();
		this.permission			= new ComboBox();
		this.instanceName		= new TextBox();
		this.instanceNameLabel	= new Label();
		this.nonce				= new TextBox();
		this.nonceLabel			= new Label();
		this.ownerId			= new TextBox();
		this.ownerIdLabel		= new Label();
		this.instanceIdLabel	= new Label();
		this.instanceId			= new Label();
		this.launchVrc			= new Button();
		this.detail				= new Button();

		this.SuspendLayout();
		curH = curW = margin;

		/*\
		|*| World ID
		\*/
		this.worldIdLabel.Text		= "World ID";
		this.worldIdLabel.AutoSize	= false;
		this.worldIdLabel.Location	= new Point(curW, curH);
		this.worldIdLabel.Size		= new Size(textBoxW, 18);
		this.worldIdLabel.Font		= new Font("Consolas", 12F);

		curH += this.worldIdLabel.Size.Height;
		curH += padding;

		this.worldId.Text			= this.instance.WorldId;
		this.worldId.Size			= new Size(textBoxW, 20);
		this.worldId.Font			= new Font("Consolas", 9F);
		this.worldId.Location		= new Point(curW, curH);
		this.worldId.TextChanged	+= new EventHandler(textBoxChanged);

		curH += this.worldId.Size.Height;
		curH += padding;

		/*\
		|*| Permission
		\*/
		this.permissionLabel.Text		= "Permission";
		this.permissionLabel.AutoSize	= false;
		this.permissionLabel.Location	= new Point(curW, curH);
		this.permissionLabel.Size		= new Size(textBoxW, 18);
		this.permissionLabel.Font		= new Font("Consolas", 12F);

		curH += this.permissionLabel.Size.Height;
		curH += padding;

		this.permission.DataSource				= Enum.GetValues(typeof(Permission));
		this.permission.DropDownStyle			= ComboBoxStyle.DropDownList;
		this.permission.Size					= new Size(textBoxW, 20);
		this.permission.Font					= new Font("Consolas", 9F);
		this.permission.Location				= new Point(curW, curH);
		this.permission.SelectedIndexChanged	+= new EventHandler(permissionChanged);

		curH += this.permission.Size.Height;
		curH += padding;

		/*\
		|*| InstanceName
		\*/
		this.instanceNameLabel.Text		= "Instance Name (invalid)";
		this.instanceNameLabel.AutoSize	= false;
		this.instanceNameLabel.Location	= new Point(curW, curH);
		this.instanceNameLabel.Size		= new Size(textBoxW, 18);
		this.instanceNameLabel.Font		= new Font("Consolas", 12F);

		curH += this.instanceNameLabel.Size.Height;
		curH += padding;

		this.instanceName.Text			= this.instance.InstanceName;
		this.instanceName.Size			= new Size(textBoxW, 20);
		this.instanceName.Font			= new Font("Consolas", 9F);
		this.instanceName.Location		= new Point(curW, curH);
		this.instanceName.TextChanged	+= new EventHandler(textBoxChanged);

		curH += this.instanceName.Size.Height;
		curH += padding;

		/*\
		|*| Owner ID
		\*/
		this.ownerIdLabel.Text		= "Owner ID (invalid)";
		this.ownerIdLabel.AutoSize	= false;
		this.ownerIdLabel.Location	= new Point(curW, curH);
		this.ownerIdLabel.Size		= new Size(textBoxW, 18);
		this.ownerIdLabel.Font		= new Font("Consolas", 12F);

		curH += this.ownerIdLabel.Size.Height;
		curH += padding;

		this.ownerId.Text			= this.instance.OwnerId;
		this.ownerId.Size			= new Size(textBoxW, 20);
		this.ownerId.Font			= new Font("Consolas", 9F);
		this.ownerId.Location		= new Point(curW, curH);
		this.ownerId.TextChanged	+= new EventHandler(textBoxChanged);

		curH += this.ownerId.Size.Height;
		curH += padding;

		/*\
		|*| Nonce
		\*/
		this.nonceLabel.Text		= "Nonce (invalid)";
		this.nonceLabel.AutoSize	= false;
		this.nonceLabel.Location	= new Point(curW, curH);
		this.nonceLabel.Size		= new Size(textBoxW, 18);
		this.nonceLabel.Font		= new Font("Consolas", 12F);
		this.nonceLabel.DoubleClick	+= new EventHandler(nonceLabelDoubleClick);

		curH += this.nonceLabel.Size.Height;
		curH += padding;

		this.nonce.Text			= this.instance.Nonce;
		this.nonce.Size			= new Size(textBoxW, 20);
		this.nonce.Font			= new Font("Consolas", 9F);
		this.nonce.Location		= new Point(curW, curH);
		this.nonce.TextChanged	+= new EventHandler(textBoxChanged);

		curH += this.nonce.Size.Height;
		curH += padding;

		/*\
		|*| Instance Id
		\*/
		this.instanceIdLabel.Text		= "Instance ID";
		this.instanceIdLabel.AutoSize	= false;
		this.instanceIdLabel.Location	= new Point(curW, curH);
		this.instanceIdLabel.Size		= new Size(textBoxW, 18);
		this.instanceIdLabel.Font		= new Font("Consolas", 12F);
		
		curH += this.instanceIdLabel.Size.Height;
		curH += padding;

		this.instanceId.Text		= "wrld_xxx:12345~public";
		this.instanceId.AutoSize	= false;
		this.instanceId.Location	= new Point(curW, curH);
		this.instanceId.Size		= new Size(textBoxW, 75);
		this.instanceId.Font		= new Font("Consolas", 9F);

		curH += this.instanceId.Size.Height;
		curH += padding;

		/*\
		|*| Buttons
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

		curW = margin;
		curH += this.launchVrc.Size.Height;

		/*\
		|*| Form
		\*/
		this.Text				= "Edit Instance - VRChat RejoinTool";
		this.ClientSize			= new Size(textBoxW + (margin * 2), curH);
		this.MinimumSize		= this.Size;
		this.MaximumSize		= this.Size;
		this.FormBorderStyle	= FormBorderStyle.FixedSingle;
		this.Icon				= new Icon(execAsm.GetManifestResourceStream("icon"));
		this.ContextMenuStrip	= instanceIdContextMenu;
		this.KeyDown			+= new KeyEventHandler(windowKeyDown);
		this.KeyPreview			= true;

		this.Controls.Add(this.worldIdLabel);
		this.Controls.Add(this.worldId);
		this.Controls.Add(this.permissionLabel);
		this.Controls.Add(this.permission);
		this.Controls.Add(this.instanceNameLabel);
		this.Controls.Add(this.instanceName);
		this.Controls.Add(this.ownerIdLabel);
		this.Controls.Add(this.ownerId);
		this.Controls.Add(this.nonceLabel);
		this.Controls.Add(this.nonce);
		this.Controls.Add(this.instanceIdLabel);
		this.Controls.Add(this.instanceId);
		this.Controls.Add(this.launchVrc);
		this.Controls.Add(this.detail);

		this.ResumeLayout(false);
	}
}

