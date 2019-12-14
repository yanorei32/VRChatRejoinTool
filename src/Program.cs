using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Program {
	static void readLogfile(FileStream fs, List<Visit> visitHistory) {
		Regex instanceRegex = new Regex(@"wrld_.+");
		Regex dateTimeRegex = new Regex(@"\d{4}(\.\d{2}){2} \d{2}(:\d{2}){2}");

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

				visitHistory.Add(new Visit(new Instance(instance), dateTime));
			}
		}
	}

	static IEnumerable<FileInfo> getLogFiles() {
		return new DirectoryInfo(
			Environment.ExpandEnvironmentVariables(
				@"%AppData%\..\LocalLow\VRChat\VRChat"
			)
		).EnumerateFiles("output_log_*.txt");
	}

	static void showMessage(string message, bool noDialog) {
		if (noDialog) {
			SystemSounds.Exclamation.Play();
		} else {
			MessageBox.Show(
				message,
				message,
				MessageBoxButtons.OK,
				MessageBoxIcon.Asterisk
			);
		}
	}

	[STAThread]
	public static void Main(string[] Args) {
		List<Visit> visitHistory = new List<Visit>();

		// Arguments
		List<string> userSelectedLogFiles = new List<string>();
		List<string> ignoreWorldIds = new List<string>();
		bool
			ignorePublic = false,
			noDialog = false,
			killVRC = false,
			noGUI = false;
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
				killVRC = true;
				continue;
			}

			if (arg == "--ignore-public") {
				ignorePublic = true;
				continue;
			}

			if (arg == "--no-dialog") {
				noDialog = true;
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

			if (!File.Exists(arg)) {
				showMessage(
					"Unknown option or invalid file.: " + arg,
					noGUI && noDialog
				);

				return;
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
					readLogfile(stream, visitHistory);
				}
			}
		} else {
			IEnumerable<FileInfo> logfiles = getLogFiles();

			if (!logfiles.Any()) {
				showMessage("Could not find VRChat log.", noGUI && noDialog);
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
					readLogfile(stream, visitHistory);
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
					v.Instance.Permission != Permission.Public
					&&
					v.Instance.Permission != Permission.Unknown
				)
			)
			&&
			(
				ignoreByTimeMins == 0
				||
				compDate < v.DateTime
			)
			&&
			!ignoreWorldIds.Contains(v.Instance.WorldId)
		).OrderByDescending(
			v => v.DateTime
		).ToList();

		if (!sortedVisitHistory.Any()) {
			showMessage("Could not find visits from VRChat log.", noGUI && noDialog);
			return;
		}

		/*\
		|*| Action
		\*/
		if (noGUI) {
			VRChat.Launch(sortedVisitHistory[0].Instance, killVRC);
		} else {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(
				new MainForm(sortedVisitHistory, killVRC)
			);
		}
	}
}

