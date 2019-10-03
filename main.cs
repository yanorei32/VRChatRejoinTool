using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class VRChatRejoinInstance {
	static Regex instanceRegex = new Regex(@"wrld_.+");
	static Regex publicInstanceRegex = new Regex(@":\d+$");
	static Regex dateTimeRegex = new Regex(@"\d{4}(\.\d{2}){2} \d{2}(:\d{2}){2}");

	static bool noGUI = false, killVrc = false;

	enum Permission { Unknown, InviteOnly, InvitePlus, Friends, FriendsPlus, Public };

	class Visit {
		string instance;
		DateTime dateTime;
		Permission permission;

		public Permission Permission {
			get { return this.permission; }
		}

		public string Instance {
			get { return this.instance; }
		}

		public string WorldId {
			get { return this.instance.Split(':')[0]; }
		}

		public DateTime DateTime {
			get { return this.dateTime; }
		}

		public Visit(string instance, string dateTime) {
			this.instance = instance;
			this.dateTime = DateTime.Parse(dateTime);

			this.permission = Permission.Unknown;

			if (instance.Contains("canRequestInvite"))
				this.permission = Permission.InvitePlus;
			else if (instance.Contains("private"))
				this.permission = Permission.InviteOnly;
			else if (instance.Contains("friends"))
				this.permission = Permission.Friends;
			else if (instance.Contains("hidden"))
				this.permission = Permission.FriendsPlus;
			else if (instance.Contains("public") || publicInstanceRegex.IsMatch(instance))
				this.permission = Permission.Public;
		}
	}

	class Form1 : Form {
		PictureBox logo;
		Button launchVrc, showInVrcw, next, prev;
		Label datetime, instance, permission;
		List<Visit> sortedHistory;
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
			launchVRChat(sortedHistory[index]);
			this.Close();
		}

		void showInVrcwButtonClick(object sender, EventArgs e) {
			Process.Start(
				"https://www.vrcw.net/world/detail/" + sortedHistory[index].WorldId
			);
		}

		void InitializeComponent() {
			const int
				imgW	= 320,
				imgH	= 84,
				margin	= 13,
				padding	= 6;

			int curW = 0, curH = 0;
			Assembly execAsm = Assembly.GetExecutingAssembly();

			this.logo		= new PictureBox();
			this.prev		= new Button();
			this.next		= new Button();
			this.launchVrc	= new Button();
			this.showInVrcw	= new Button();
			this.datetime	= new Label();
			this.instance	= new Label();
			this.permission	= new Label();

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
			this.instance.Font		= new Font("Conolas", 9F);

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

			this.showInVrcw.Text		= "Detail";
			this.showInVrcw.Location	= new Point(curW, curH);
			this.showInVrcw.Size		= new Size(75, 23);
			this.showInVrcw.Click		+= new EventHandler(showInVrcwButtonClick);

			curW = margin;
			curH += this.launchVrc.Size.Height;
			curH += margin;

			/*\
			|*| Form
			\*/
			this.Text				= "VRChat RejoinTool";
			this.ClientSize			= new Size(imgW + (margin * 2), curH);
			this.MinimumSize		= this.Size;
			this.MaximumSize		= this.Size;
			this.FormBorderStyle	= FormBorderStyle.FixedSingle;
			this.Icon				= new Icon(execAsm.GetManifestResourceStream("icon"));
			this.Controls.Add(this.logo);
			this.Controls.Add(this.launchVrc);
			this.Controls.Add(this.showInVrcw);
			this.Controls.Add(this.prev);
			this.Controls.Add(this.next);
			this.Controls.Add(this.datetime);
			this.Controls.Add(this.instance);
			this.Controls.Add(this.permission);
			this.ResumeLayout(false);
		}

		void update() {
			Visit v = sortedHistory[index];

			this.instance.Text = "Instance:\n" + v.Instance;
			this.datetime.Text = "Date: " + v.DateTime.ToString();
			this.permission.Text = "Permission: " + Enum.GetName(typeof(Permission), v.Permission);
			this.prev.Enabled = 1 <= index;
			this.next.Enabled = index <= sortedHistory.Count - 2;
		}

		public Form1(List<Visit> sortedHistory) {
			this.sortedHistory = sortedHistory;
			InitializeComponent();
			update();
		}
	}

	static void ReadLogfile(FileStream fs, List<Visit> visitHistory) {
		using (
			StreamReader reader = new StreamReader(fs)
		) {
			string lineString, dateTime, instance;
			Match match;

			while ((lineString = reader.ReadLine()) != null) {
				if (!lineString.Contains("[VRCFlowManagerVRC] Destination set: "))
					continue;

				match = instanceRegex.Match(lineString);
				if (!match.Success) continue;
				instance = match.Value;

				match = dateTimeRegex.Match(lineString);
				if (!match.Success) continue;
				dateTime = match.Value;

				visitHistory.Add(new Visit(instance, dateTime));
			}
		}
	}

	static IEnumerable<FileInfo> getLogFiles() {
		return new DirectoryInfo(
			Environment.ExpandEnvironmentVariables(
				@"%AppData%\..\LocalLow\VRChat\VRChat"
			)
		).EnumerateFiles(
			"output_log_*.txt"
		);
	}

	static void ShowMessage(string message, bool noGUI) {

		MessageBox.Show(
			message,
			message,
			MessageBoxButtons.OK,
			MessageBoxIcon.Asterisk
		);
	}

	static void launchVRChat(Visit v) {
		if (killVrc)
			foreach (var p in Process.GetProcessesByName("vrchat"))
				p.Kill();

		Process.Start("vrchat://launch?id=" + v.Instance);
	}

	public static void Main(string[] Args) {
		List<Visit> visitHistory = new List<Visit>();
		List<string> userSelectedLogFiles = new List<string>();
		List<string> ignoreWorldIds = new List<string>();
		bool ignorePublic = false;
		int ignoreByTimeMins = 0;

		Match match;

		Regex ignoreWorldsArgRegex = new Regex(@"--ignore-worlds=wrld_.+(,wrld_.+)?");
		Regex ignoreByTimeRegex = new Regex(@"--ignore-by-time=\d+");

		/*\
		|*| Parse arguments
		\*/
		foreach (string arg in Args) {
			if (arg == "--no-gui") {
				noGUI = true;
				continue;
			}

			if (arg == "--kill-vrc") {
				killVrc = true;
				continue;
			}

			if (arg == "--ignore-public") {
				ignorePublic = true;
				continue;
			}

			match = ignoreWorldsArgRegex.Match(arg);
			if (match.Success) {
				foreach (string world in arg.Split('=')[1].Split(','))
					ignoreWorldIds.Add(world);

				continue;
			}

			match = ignoreByTimeRegex.Match(arg);
			if (match.Success) {
				ignoreByTimeMins = int.Parse(arg.Split('=')[1]);
				continue;
			}

			userSelectedLogFiles.Add(arg);
		}


		/*\
		|*| Read logfiles
		|*|  (Find logfiles by AppData if userSelectedLogFiles is empty)
		\*/
		if (userSelectedLogFiles.Any()) {
			foreach (string filepath in userSelectedLogFiles) {
				using (
					FileStream stream = new FileStream(
						filepath,
						FileMode.Open,
						FileAccess.Read,
						FileShare.ReadWrite
					)
				) {
					ReadLogfile(stream, visitHistory);
				}
			}
		} else {
			IEnumerable<FileInfo> logfiles = getLogFiles();

			if (!logfiles.Any()) {
				ShowMessage("Could not find VRChat log.", noGUI);
				return;
			}

			foreach (FileInfo logfile in logfiles) {
				using (
					FileStream stream = logfile.Open(
						FileMode.Open,
						FileAccess.Read,
						FileShare.ReadWrite
					)
				) {
					ReadLogfile(stream, visitHistory);
				}
			}
		}


		/*\
		|*| Filter and Sort
		\*/
		DateTime compDate = DateTime.Now.AddMinutes(ignoreByTimeMins * -1);
		List<Visit> sortedVisitHistory = visitHistory.Where(
			v => (
				!ignorePublic
				||
				(
					v.Permission != Permission.Public
					&&
					v.Permission != Permission.Unknown
				)
			)
			&&
			(
				ignoreByTimeMins == 0
				||
				compDate < v.DateTime
			)
			&&
			!ignoreWorldIds.Contains(v.WorldId)
		).OrderByDescending(
			v => v.DateTime
		).ToList();

		if (!sortedVisitHistory.Any()) {
			ShowMessage("Could not find visits from VRChat log.", noGUI);
			return;
		}

		/*\
		|*| Action
		\*/
		if (noGUI) {
			launchVRChat(sortedVisitHistory[0]);
		} else {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(
				new Form1(sortedVisitHistory)
			);
		}
	}
}

