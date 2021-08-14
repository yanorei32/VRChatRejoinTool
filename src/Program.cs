using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Program {
	static void readLogfile(FileStream fs, List<Visit> visitHistory) {
		Regex instanceRegex = new Regex(@"wr?ld_.+");
		Regex dateTimeRegex = new Regex(@"\d{4}(\.\d{2}){2} \d{2}(:\d{2}){2}");

		using (
			StreamReader reader = new StreamReader(fs)
		) {
			const int CLEARED				= 0;
			const int DESTINATION_SET_FOUND	= 1 << 0;
			const int WORLD_NAME_FOUND		= 1 << 1;

			int state = CLEARED;

			string lineString, dateTime = "", instance = "", worldName = "";
			Match match;

			while ((lineString = reader.ReadLine()) != null) {
				if (lineString.Contains("[Behaviour] Destination set: w")) {
					// Push current instance if not cleared.
					if ((state & DESTINATION_SET_FOUND) != 0)
						visitHistory.Add(new Visit(
							new Instance(
								instance,
								(state & WORLD_NAME_FOUND) != 0 ? worldName : null
							),
							dateTime
						));

					state = CLEARED;

					match = instanceRegex.Match(lineString);

					if (!match.Success) continue;
					instance = match.Value;

					match = dateTimeRegex.Match(lineString);
					if (!match.Success) continue;
					dateTime = match.Value;

					state = DESTINATION_SET_FOUND;

					continue;
				}

				if (lineString.Contains("[Behaviour] Joining w")) {
					match = instanceRegex.Match(lineString);

					if (!match.Success) continue;
					instance = match.Value;

					continue;
				}

				if (
					lineString.Contains("[Behaviour] Entering Room: ")
					||
					lineString.Contains("[Behaviour] Joining or Creating Room: ")
				) {
					worldName = lineString.Split(':')[3].TrimStart();
					state |= WORLD_NAME_FOUND;
				}
			}

			// Push current instance if not cleared.
			if ((state & DESTINATION_SET_FOUND) != 0)
				visitHistory.Add(new Visit(
					new Instance(
						instance,
						(state & WORLD_NAME_FOUND) != 0 ? worldName : null
					),
					dateTime
				));
		}
	}

	static string FindInPath(string filename) {
		var path = Path.Combine(
			Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString(),
			filename
		);

		if (File.Exists(path)) return path;

		foreach (var dir in Environment.GetEnvironmentVariable("PATH").Split(';')) {
			path = Path.Combine(dir, filename);
			if (File.Exists(path)) return path;
		}

		foreach (var dir in Environment.GetEnvironmentVariable("PATHEXT").Split(';')) {
			path = Path.Combine(dir, filename);
			if (File.Exists(path)) return path;
		}

		return null;
	}

	static IEnumerable<FileInfo> getLogFiles() {
		string path = Environment.ExpandEnvironmentVariables(
			@"%AppData%\..\LocalLow\VRChat\VRChat"
		);

		if (!Directory.Exists(path)) {
			return null;
		}

		return new DirectoryInfo(path).EnumerateFiles("output_log_*.txt");
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
		var visitHistory = new List<Visit>();

		// Arguments
		var userSelectedLogFiles = new List<string>();
		var ignoreWorldIds = new List<string>();

		string vrcInviteMePath = FindInPath("vrc-invite-me.exe");

		bool
			ignorePublic	= false,
			noDialog		= false,
			killVRC			= false,
			noGUI			= false,
			quickSave		= false,
			quickSaveHTTP	= false,
			inviteMe		= false;

		int
			ignoreByTimeMins	= 0,
			index				= 0;

		Match match;
		var ignoreWorldsArgRegex	= new Regex(@"\A--ignore-worlds=wrld_.+(,wrld_.+)?\z");
		var ignoreByTimeRegex		= new Regex(@"\A--ignore-by-time=\d+\z");
		var indexRegex				= new Regex(@"\A--index=\d+\z");

		/*\
		|*| Parse arguments
		\*/
		foreach (var arg in Args) {
			if (arg == "--quick-save") {
				quickSave = true;
				continue;
			}

			if (arg == "--quick-save-http") {
				quickSaveHTTP = true;
				continue;
			}

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

			if (arg == "--invite-me") {
				inviteMe = true;
				continue;
			}

			match = indexRegex.Match(arg);
			if (match.Success) {
				index = int.Parse(arg.Split('=')[1]);
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

		if (inviteMe && vrcInviteMePath == null) {
			showMessage("Failed to find vrc-invite-me.exe", noGUI && noDialog);
			return;
		}

		if (quickSave && quickSaveHTTP) {
			showMessage(
				"The combination of --quick-save and --quick-save-http cannot be used.",
				noGUI && noDialog
			);

			return;
		}


		/*\
		|*| Read logfiles
		|*|  (Find logfiles by AppData if userSelectedLogFiles is empty)
		\*/
		if (userSelectedLogFiles.Any()) {
			foreach (var filepath in userSelectedLogFiles) {
				using (
					var stream = new FileStream(
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
			IEnumerable<FileInfo> logFiles = getLogFiles();

			if (logFiles == null) {
				showMessage("Failed to lookup VRChat log directory.", noGUI && noDialog);
				return;
			}

			if (!logFiles.Any()) {
				showMessage("Could not find VRChat log.", noGUI && noDialog);
				return;
			}

			foreach (var logFile in logFiles) {
				using (
					var stream = logFile.Open(
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
		var compDate = DateTime.Now.AddMinutes(ignoreByTimeMins * -1);
		List<Visit> sortedVisitHistory = visitHistory.Where(
			v => (
				!ignorePublic
				||
				(
					v.Instance.Permission != Permission.Public
					&&
					v.Instance.Permission != Permission.PublicWithIdentifier
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
			if (index >= sortedVisitHistory.Count) {
				showMessage("Out of bounds index: " + index.ToString(), noDialog);
				return;
			}

			var v = sortedVisitHistory[index];
			var i = v.Instance;

			if (quickSave || quickSaveHTTP) {
				string saveDir = Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location
				) + @"\saves";

				try {
					if (!Directory.Exists(saveDir))
						Directory.CreateDirectory(saveDir);
				} catch (Exception e) {
					showMessage(
						"[QuickSave] Make Directory:\n" + e.Message,
						noDialog
					);

					throw e;
				}

				string instanceString = i.WorldId;
				string idWithoutWorldId = i.IdWithoutWorldId;

				if (idWithoutWorldId != "") {
					instanceString += "-";
					instanceString += idWithoutWorldId;
				}

				var existsLinks = new DirectoryInfo(saveDir).EnumerateFiles(
					quickSaveHTTP ? "web-*.lnk" : "*.lnk"
				);

				foreach (FileInfo link in existsLinks) {
					if (quickSave && link.Name.StartsWith("web-")) {
						continue;
					}

					if (link.Name.EndsWith(instanceString + ".lnk")) {
						File.SetLastWriteTime(link.FullName, DateTime.Now);
						return;
					}
				}

				var filePath = string.Format(
					@"{0}\{3}{1}{2}.lnk",
					saveDir,
					v.DateTime.ToString("yyyyMMdd-hhmmss-"),
					instanceString,
					quickSaveHTTP ? "web-" : string.Empty
				);

				try {
					VRChat.SaveInstanceToShortcut(i, filePath, quickSaveHTTP);
				} catch (Exception e) {
					showMessage(
						"[QuickSave] Create Shortcut:\n" + e.Message,
						noDialog
					);

					throw e;
				}
			} else if (inviteMe) {
				if (VRChat.InviteMe(i, vrcInviteMePath) != 0) {
					showMessage(
						"Check your vrc-invite-me.exe settings",
						noDialog
					);
				}
			} else {
				VRChat.Launch(i, killVRC);
			}

		} else {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(
				new MainForm(sortedVisitHistory, killVRC, vrcInviteMePath)
			);

		}
	}
}

