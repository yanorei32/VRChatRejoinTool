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
	static Regex dateTimeRegex = new Regex(@"\d{4}(\.\d{2}){2} \d{2}(:\d{2}){2}");

	class Visit {
		string instance;
		string dateTime;

		public string Instance {
			get { return this.instance; }
		}

		public string DateTime {
			get { return this.dateTime; }
		}

		public Visit(string instance, string dateTime) {
			this.instance = instance;
			this.dateTime = dateTime;
		}
	}

	class Form1 : Form {
		static Regex publicInstanceRegex = new Regex(@":\d+$");
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
			Process.Start(
				"vrchat://launch?id=" + sortedHistory[index].Instance
			);

			this.Close();
		}

		void showInVrcwButtonClick(object sender, EventArgs e) {
			Process.Start(
				"https://www.vrcw.net/world/detail/" + sortedHistory[index].Instance.Split(':')[0]
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
			string permission = "Unknown";
			Visit v = sortedHistory[index];

			if (v.Instance.Contains("canRequestInvite")) {
				permission = "Invite+";
			} else if (v.Instance.Contains("private")) {
				permission = "Ivite Only";
			} else if (v.Instance.Contains("friends")) {
				permission = "Friends";
			} else if (v.Instance.Contains("hidden")) {
				permission = "Friends+";
			} else if (v.Instance.Contains("public") || publicInstanceRegex.IsMatch(v.Instance)) {
				permission = "Public";
			}

			this.instance.Text = "Instance:\n" + v.Instance;
			this.datetime.Text = "Date: " + v.DateTime;
			this.permission.Text = "Permission: " + permission;
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

	static FileInfo getNewerLogFile() {
		return new DirectoryInfo(
			Environment.ExpandEnvironmentVariables(
				@"%AppData%\..\LocalLow\VRChat\VRChat"
			)
		).EnumerateFiles(
			"output_log_*.txt"
		).OrderByDescending(
			f => f.CreationTime
		).FirstOrDefault();
	}

	public static void Main(string[] Args) {
		List<Visit> visitHistory = new List<Visit>();

		if (Args.Length == 0) {
			FileInfo newerLogFile = getNewerLogFile();

			if (newerLogFile == null) {
				MessageBox.Show(
					"Could not find VRChat log.",
					"Could not find VRChat log.",
					MessageBoxButtons.OK,
					MessageBoxIcon.Asterisk
				);

				return;
			}

			using (
				FileStream stream = newerLogFile.Open(
					FileMode.Open,
					FileAccess.Read,
					FileShare.ReadWrite
				)
			) {
				ReadLogfile(stream, visitHistory);
			}
		} else {
			foreach (string filepath in Args) {
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
		}

		if (visitHistory.Count == 0) {
			MessageBox.Show(
				"Could not find last visited instance.",
				"Could not find last visited instance.",
				MessageBoxButtons.OK,
				MessageBoxIcon.Asterisk
			);

			return;
		}

		visitHistory.Sort(
			(a, b) => string.Compare(b.DateTime, a.DateTime)
		);

		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(
			new Form1(visitHistory)
		);
	}
}

