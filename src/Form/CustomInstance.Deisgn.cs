using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

partial class CustonmInstanceForm : Form {
	void initializeComponent() {
		const int
			textBoxW = 320,
			margin	= 13,
			padding	= 6;

		int curW = 0, curH = 0;

		this.worldIdLabel		= new Label();
		this.worldId			= new Text();
		this.permissionLabel	= new Label();
		this.permission			= new Text();
		this.visibleIdLabel		= new Label();
		this.visibleId			= new Text();
		this.ownerLabel			= new Label();
		this.owner				= new Text();
		this.nonceLabel			= new Label();
		this.nonce				= new Text();
		this.copyLaunchURI		= new Button();
		this.copyPrettyURI		= new Button();
		this.statusIndicator	= new Label();
		this.saveBatFile		= new Button();

		this,SuspendLayout();
		curH = curW = margin;

		/*\
		|*| World ID Label Column
		\*/

		this.worldIdLabel.Location		= new Point(curH, curW);
		this.worldIdLabel.Text			= "World Id (wrld_xxx)";
		this.worldIdLabel.Size			= new Size(

	}
}
