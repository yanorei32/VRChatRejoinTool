using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VRChatRejoinTool.Form;
using static VRChatRejoinTool.FunctionalPiece;

namespace VRChatRejoinTool {
	static class Program {
		static void readLogfile(FileStream fs, List<Visit> visitHistory) {
			var instanceRegex = new Regex(@"wr?ld_.+");
			var dateTimeRegex = new Regex(@"\d{4}(\.\d{2}){2} \d{2}(:\d{2}){2}");
		
			using (
				var reader = new StreamReader(fs)
			) {
				var state = LogParseState.Cleared;

				string lineString, dateTime = "", instance = "", worldName = "";

				// TODO: lazy read lines?
				while ((lineString = reader.ReadLine()) != null) {
					if (lineString.Contains("[Behaviour] Destination set: w")) {
						// Push current instance if not cleared.
						if ((state & LogParseState.DestinationSetFound) != 0)
							visitHistory.Add(new Visit(
								new Instance(
									instance,
									(state & LogParseState.WorldNameFound) != 0 ? worldName : null
								),
								dateTime
							));

						state = LogParseState.Cleared;

						// FIXME: 名前がよくなさそう
						var proceedParse = true;
						proceedParse = proceedParse && actionIfMatch(lineString, instanceRegex, s => {
							instance = s;
						});

						proceedParse = proceedParse && actionIfMatch(lineString, dateTimeRegex, s => {
							dateTime = s;
						});

						if (!proceedParse) {
							state = LogParseState.DestinationSetFound;
						}

						continue;
					}

					if (lineString.Contains("[Behaviour] Joining w")) {
						actionIfMatch(lineString, instanceRegex, s => {
							instance = s;
						});
						continue;
					}

					if (
						lineString.Contains("[Behaviour] Entering Room: ")
						||
						lineString.Contains("[Behaviour] Joining or Creating Room: ")
					) {
						worldName = lineString.Split(':')[3].TrimStart();
						state |= LogParseState.WorldNameFound;
					}
				}

				// Push current instance if not cleared.
				if ((state & LogParseState.DestinationSetFound) != 0)
					visitHistory.Add(new Visit(
						new Instance(
							instance,
							(state & LogParseState.WorldNameFound) != 0 ? worldName : null
						),
						dateTime
					));
			}
		}

		static string FindInPath(string filename) {
			{
				var path = Path.Combine(
					Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString(),
					filename
				);

				if (File.Exists(path)) return path;
			}

			// ReSharper disable once PossibleNullReferenceException
			// This is safe on Windows: PATH must be exist
			// FIXME: this logic is not correct because separator is not same:.
			// Windows is semi-colon, non-Windows environment (includes WSL) is colon.
			// also note for future implementation: ext4 allows mostly ANY character.
			foreach (var dir in Environment.GetEnvironmentVariable("PATH").Split(';')) {
				var path = Path.Combine(dir, filename);
				if (File.Exists(path)) return path;
			}

			var pathExt = Environment.GetEnvironmentVariable("PATHEXT");
			// on Windows, it is `true` always. It may not exist on other platform.
			if (pathExt != null) {
				foreach (var dir in pathExt.Split(';')) {
					var path = Path.Combine(dir, filename);
					if (File.Exists(path)) return path;
				}
			}

			return null;
		}

		/**
		 * <summary>VRChatのログファイルを格納するディレクトリを探索する。</summary>
		 * <returns>ディレクトリが見つかればそのディレクトリ内に入っているファイル。見つからなければnull。</returns>
		 */
		static ICollection<FileInfo> getLogFiles() {
			string path = Environment.ExpandEnvironmentVariables(
				@"%AppData%\..\LocalLow\VRChat\VRChat"
			);

			return Directory.Exists(path)
				? new DirectoryInfo(path).EnumerateFiles("output_log_*.txt").ToList()
				: null;
		}

		/**
		 * <summary>警告を指定した方法で表示する。</summary>
		 * <param name="showDialog"><c>true</c>ならば画面を表示する。<c>false</c>ならば画面を表示しない。</param>
		 * <param name="lazyMessage">遅延評価で表示されるメッセージ</param>
		 */
		static void showMessage(bool showDialog, Func<string> lazyMessage) {
			if (showDialog) {
				var message = lazyMessage();
				MessageBox.Show(
					message,
					message,
					MessageBoxButtons.OK,
					MessageBoxIcon.Asterisk
				);
			} else {
				SystemSounds.Exclamation.Play();
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
				ignoreByTimeMins	= 0, // 0 means don't ignore
				index				= 0;

			var ignoreWorldsArgRegex	= new Regex(@"\A--ignore-worlds=wrld_.+(,wrld_.+)?\z");
			var ignoreByTimeRegex		= new Regex(@"\A--ignore-by-time=\d+\z");
			var indexRegex				= new Regex(@"\A--index=\d+\z");

			/*\
			|*| Parse arguments
			\*/
			foreach (var arg in Args) {
				switch (arg)
				{
					case "--quick-save":
						quickSave = true;
						continue;
					case "--quick-save-http":
						quickSaveHTTP = true;
						continue;
					case "--no-gui":
						noGUI = true;
						continue;
					case "--kill-vrc":
						killVRC = true;
						continue;
					case "--ignore-public":
						ignorePublic = true;
						continue;
					case "--no-dialog":
						noDialog = true;
						continue;
					case "--invite-me":
						inviteMe = true;
						continue;
				}

				// continueがdelegateから外側に使えないため、代用のフラグ変数
				var dontParse = false;

				// NOTE: ||= isn't available as of C# 4.0
				dontParse = dontParse || actionIfMatch(arg, indexRegex, () => {
					index = int.Parse(arg.Split('=')[1]);
				});
				dontParse = dontParse || actionIfMatch(arg, ignoreWorldsArgRegex, () => {
					foreach (string world in arg.Split('=')[1].Split(','))
						ignoreWorldIds.Add(world);
				});
				dontParse = dontParse || actionIfMatch(arg, ignoreByTimeRegex, () => {
					ignoreByTimeMins = int.Parse(arg.Split('=')[1]);
				});

				if (dontParse) continue;
				if (!File.Exists(arg)) {
					showMessage(
						!(noGUI && noDialog), 
						() => "Unknown option or invalid file.: " + arg);

					return;
				}

				userSelectedLogFiles.Add(arg);
			}

			if (inviteMe && vrcInviteMePath == null) {
				showMessage(!(noGUI && noDialog), () => "Failed to find vrc-invite-me.exe");
				return;
			}

			if (quickSave && quickSaveHTTP) {
				showMessage(
					!(noGUI && noDialog), 
					() => "The combination of --quick-save and --quick-save-http cannot be used.");

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
				// !files.Any()をしたあとにfor-eachするとRiderから警告が出るのでそれを黙らせる
				var logFiles = getLogFiles();

				if (logFiles == null) {
					showMessage(!(noGUI && noDialog), () => "Failed to lookup VRChat log directory.");
					return;
				}

				if (logFiles.Count == 0) {
					showMessage(!(noGUI && noDialog), () => "Could not find VRChat log.");
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
			var threshold = DateTime.Now.AddMinutes(ignoreByTimeMins * -1);
			var dontFilterByTime = ignoreByTimeMins == 0;
			bool permissionSelector(Permission p) => p != Permission.Public
			                                        &&
													p != Permission.Unknown;

			List<Visit> sortedVisitHistory = visitHistory.Where(
				v => (
							!ignorePublic
							||
							permissionSelector(v.Instance.Permission)
						)
						&&
						(
							dontFilterByTime || threshold < v.DateTime
						)
						&&
						!ignoreWorldIds.Contains(v.Instance.WorldId)
			).OrderByDescending(
				v => v.DateTime
			).ToList();

			if (!sortedVisitHistory.Any()) {
				showMessage(!(noGUI && noDialog), () => "Could not find visits from VRChat log.");
				return;
			}

			/*\
			|*| Action
			\*/
			if (noGUI) {
				if (index >= sortedVisitHistory.Count) {
					showMessage(!noDialog, () => "Out of bounds index: " + index);
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
							!noDialog,
							() => "[QuickSave] Make Directory:\n" + e.Message
						);

						throw;
					}

					string instanceString = i.WorldId;
					string idWithoutWorldId = i.IdWithoutWorldId;

					if (idWithoutWorldId != "") {
						instanceString += "-";
						instanceString += idWithoutWorldId;
					}

					var shortcuts = new DirectoryInfo(saveDir).EnumerateFiles(
						quickSaveHTTP ? "web-*.lnk" : "*.lnk"
					);

					foreach (FileInfo shortcut in shortcuts) {
						if (quickSave && shortcut.Name.StartsWith("web-")) {
							continue;
						}

						if (shortcut.Name.EndsWith(instanceString + ".lnk")) {
							File.SetLastWriteTime(shortcut.FullName, DateTime.Now);
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
							!noDialog, 
							() => "[QuickSave] Create Shortcut:\n" + e.Message
						);

						throw;
					}
				} else if (inviteMe) {
					if (VRChat.InviteMe(i, vrcInviteMePath) != 0) {
						showMessage(
							!noDialog, 
							() => "Check your vrc-invite-me.exe settings"
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
}

