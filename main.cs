using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		foreach (Visit v in visitHistory.OrderByDescending(v_ => v_.DateTime)) {
			switch (
				MessageBox.Show(
					String.Format(
						"Do you want to return to \n\n{0}\n\n{1} ?",
						v.DateTime,
						v.Instance
					),
					"Found it.",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				)
			) {
				case DialogResult.Yes:
					Process.Start(
						"vrchat://launch?id=" + v.Instance
					);

					return;

				case DialogResult.No:
					continue;

				case DialogResult.Cancel:
					return;
			}
		}
	}
}

