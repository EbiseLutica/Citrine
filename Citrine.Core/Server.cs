using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Citrine.Core.Api;
using Citrine.Core.Modules;

namespace Citrine.Core
{
    /// <summary>
    /// Citrine's Core.
    /// </summary>
    public class Server
	{
		public static string CitrineAA => 
@" _____  _  _          _              
/  __ \(_)| |        (_)             
| /  \/ _ | |_  _ __  _  _ __    ___ 
| |    | || __|| '__|| || '_ \  / _ \
| \__/\| || |_ | |   | || | | ||  __/
 \____/|_| \__||_|   |_||_| |_| \___|";

		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "5.4.0";

		[Obsolete("6.0.0で廃止されます。 " + nameof(Version) + " を使用してください。")]
		public static string VersionAsXelticaBot => Version;

		/// <summary>
		/// 読み込まれているモジュール一覧を取得します。
		/// </summary>
		public List<ModuleBase> Modules { get; }

		/// <summary>
		/// 読み込まれているコマンド一覧を取得します。
		/// </summary>
		public List<CommandBase> Commands { get; }

		/// <summary>
		/// シェルを取得します。
		/// </summary>
		public IShell Shell { get; }

		public Logger Logger => new Logger("Core");

		/// <summary>
		/// 文脈の一覧を取得します。
		/// </summary>
		public Dictionary<string, (ModuleBase, Dictionary<string, object>)> ContextPostDictionary { get; } = new Dictionary<string, (ModuleBase, Dictionary<string, object>)>();

		/// <summary>
		/// ユーザーの一覧を取得します。
		/// </summary>
		public Dictionary<string, (ModuleBase, Dictionary<string, object>)> ContextUserDictionary { get; } = new Dictionary<string, (ModuleBase, Dictionary<string, object>)>();

		/// <summary>
		/// ニックネームの辞書を取得します。
		/// </summary>
		/// <value></value>
		public Dictionary<string, string> NicknameMap { get; }

		/// <summary>
		/// ユーザーストレージを取得します。
		/// </summary>
		/// <returns></returns>
		public UserStorage Storage { get; } = new UserStorage();

		static Server()
		{
			Http.DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 Citrine/{Server.Version} (https://github.com/xeltica/citrine) .NET/{Environment.Version}");
		}

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		public Server(IShell shell)
		{
			Shell = shell;
			Modules = Assembly.GetExecutingAssembly().GetTypes()
						.Where(a => a.IsSubclassOf(typeof(ModuleBase)))
						.Select(a => Activator.CreateInstance(a) as ModuleBase)
						.OrderBy(mod => mod.Priority)
						.ToList();

			Commands = Assembly.GetExecutingAssembly().GetTypes()
						.Where(a => a.IsSubclassOf(typeof(CommandBase)))
						.Select(a => Activator.CreateInstance(a) as CommandBase)
						.ToList();

			if (File.Exists("./admin"))
			{
				// マイグレ
				Logger.Warn("管理者名の古い保存形式を使用しています。コンフィグファイルへのマイグレーションを開始します。");
				adminId = File.ReadAllText("./admin").Trim().ToLower();
				Config.Instance.Admin = adminId;
				Config.Instance.Save();
				File.Delete("./admin");
				Logger.Info("管理者名のデータを移行しました。");
			}
			if (string.IsNullOrEmpty(Config.Instance.Admin))
			{
				Console.Write("Admin's ID > ");
				adminId = Console.ReadLine().Trim().ToLower();
				Config.Instance.Admin = adminId;
				Config.Instance.Save();
			}

			Logger.Info($"管理者はID {Config.Instance.Admin ?? "null"}。");

			if (Config.Instance.Moderators.Count > 0)
			{
				Logger.Info($"モデレーターは {string.Join(", ", Config.Instance.Moderators)}。");
			}
			else
			{
				Logger.Info("モデレーターは いません。");
			}

			if (File.Exists("./nicknames"))
			{
				// マイグレ
				Logger.Warn("古いニックネーム保存形式を使用しています。新しい UserStorage へのマイグレーションを開始します。");
				var lines = File.ReadAllLines("./nicknames");
				lines.Select(l => {
					var kv = l.Split(',');
					return new KeyValuePair<string, string>(kv[0], string.Concat(kv.Skip(1)));
				})
				.ForEach(kv => Storage[kv.Key].Set(StorageKey.Nickname, kv.Value));
				File.Delete("./nicknames");
				Logger.Info($"{lines.Length} 人のニックネームを、新しい UserStorage に移行しました!");
			}
		}

		/// <summary>
		/// モジュールを追加します。
		/// </summary>
		public void AddModule(ModuleBase mod)
		{
			if (Modules.Contains(mod))
				return;
			Modules?.Add(mod);
		}

		/// <summary>
		/// コマンドを追加します。
		/// </summary>
		public void AddCommand(CommandBase cmd)
		{
			if (Commands.Contains(cmd))
				return;
			Commands.Add(cmd);
		}

		public CommandBase TryGetCommand(string n) => Commands.FirstOrDefault(c =>
		{
			var cn = c.Name;
			var cnl = c.Name.ToLowerInvariant();
			var nl = n.ToLowerInvariant();
			var nameIsMatch = c.IgnoreCase ? (cnl == nl) : cn == n;
			var lowerAliases = c.Aliases?.Select(a => a.ToLowerInvariant());
			return c.Aliases == default ? nameIsMatch : nameIsMatch || (c.IgnoreCase ? lowerAliases.Contains(nl) : c.Aliases.Contains(n));
		});

		public async Task<string> ExecCommand(ICommandSender sender, string command)
		{

			if (command == null)
				throw new ArgumentNullException(nameof(command));
			if (command.StartsWith("/"))
				command = command.Substring(1).Trim();
			var splitted = Regex.Split(command, @"\s").Where(s => !string.IsNullOrWhiteSpace(s));
			var name = splitted.First();
			var cmd = TryGetCommand(name);
			if (cmd == default)
				throw new NoSuchCommandException();

			if (cmd.Permission.HasFlag(PermissionFlag.AdminOnly) && !sender.IsAdmin)
				throw new AdminOnlyException();

			if (sender is PostCommandSender p)
			{
				if (cmd.Permission.HasFlag(PermissionFlag.LocalOnly) && !string.IsNullOrEmpty(p.User.Host))
					throw new LocalOnlyException();

				if (cmd.Permission.HasFlag(PermissionFlag.RemoteOnly) && string.IsNullOrEmpty(p.User.Host))
					throw new RemoteOnlyException();
			}

			try
			{
				return await cmd.OnActivatedAsync(sender, this, Shell, splitted.Skip(1).ToArray(), command.Substring(name.Length).Trim());
			}
			catch (CommandException)
			{
				return cmd.Usage;
			}
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public Task<string> ExecCommand(string command)
		{
			return ExecCommand(InternalCommandSender.Instance, command);
		}

		/// <summary>
		/// Admin 権限としてコマンドを実行します。
		/// </summary>
		public Task<string> SudoCommand(string command)
		{
			return ExecCommand(SuperInternalCommandSender.Instance, command);
		}

		/// <summary>
		/// 指定したユーザーがローカルユーザーであるかどうかを取得します。
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool IsLocal(IUser user) => user.Host == "";

		
		[Obsolete("Use " + nameof(IsSuperUser) + " instead")]
		public bool IsAdmin(IUser user) => IsSuperUser(user);

		/// <summary>
		/// 指定したユーザーが管理者またはモデレーターであるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者かモデレーターであれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsSuperUser(IUser user) => IsAdministrator(user) || IsModerator(user);

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdministrator(IUser user) => Config.Instance.Admin == user.Name && IsLocal(user);

		/// <summary>
		/// 指定したユーザーがモデレーターであるかどうかを取得します。
		/// </summary>mi
		/// <returns>モデレーターであれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsModerator(IUser user) => Config.Instance.Moderators?.Contains(user.Name) ?? false && IsLocal(user);

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(IUser user) => GetRatingOf(user.Id);

		public Rating GetRatingOf(string user)
		{
			var r = GetRatingValueOf(user);
			return r < -3  ? Rating.Hate :
			       r < 4  ? Rating.Normal :
				   r < 8 ? Rating.Like : 
				   r < 20 ? Rating.BestFriend : Rating.Partner;
		}

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public int GetRatingValueOf(IUser user) => Storage[user].Get(StorageKey.Rating, 0);

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public int GetRatingValueOf(string id) => Storage[id].Get(StorageKey.Rating, 0);

		/// <summary>
		/// ユーザーに対する好感度を上げます。
		/// </summary>
		public void Like(string userId, int amount = 1)
		{
			SetRatingValueOf(userId, GetRatingValueOf(userId) + amount);
		}
		
		/// <summary>
		/// 指定したユーザーの好感度を設定します。
		/// </summary>
		public void SetRatingValueOf(string userId, int value)
		{
			Storage[userId].Set(StorageKey.Rating, value);
		}

		/// <summary>
		/// 指定したユーザーの好感度を設定します。
		/// </summary>
		public void SetRatingValueOf(IUser user, int value)
		{
			Storage[user.Id].Set(StorageKey.Rating, value);
		}

		/// <summary>
		/// ユーザーに対する好感度を下げます。
		/// </summary>
		public void Dislike(string userId, int amount = 1) => Like(userId, -amount);

		/// <summary>
		/// ユーザーのニックネームを取得します。
		/// </summary>
		public string GetNicknameOf(IUser user) => Storage[user].Get(StorageKey.Nickname,  $"{user.Name}さん");

		/// <summary>
		/// ユーザーのニックネームを設定します。
		/// </summary>
		public void SetNicknameOf(IUser user, string name)
		{
			Storage[user].Set(StorageKey.Nickname, name);
		}

		/// <summary>
		/// ユーザーのニックネームを破棄します。
		/// </summary>
		public void ResetNicknameOf(IUser user)
		{
			Storage[user].Clear(StorageKey.Nickname);
		}

		public async Task HandleMentionAsync(IPost mention)
		{
			await Task.Delay(400);

			if (mention.IsReply && ContextPostDictionary.ContainsKey(mention.Reply.Id))
			{
				var (mod, arg) = ContextPostDictionary[mention.Reply.Id];
				await mod.OnRepliedContextually(mention, mention.Reply, arg, Shell, this);
				ContextPostDictionary.Remove(mention.Reply.Id);
				return;
			}

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.ActivateAsync(mention, Shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
                    await Shell.ReplyAsync(mention, $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}", "ん...何の話してたんだっけ...?　(エラーが発生したようです。)");
					break;
				}
			}
		}

		public async Task HandleTimelineAsync(IPost post)
		{
			await Task.Delay(400);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnTimelineAsync(post, Shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public async Task HandleDmAsync(IPost post)
		{
			await Task.Delay(400);

			if (ContextUserDictionary.ContainsKey(post.User.Id))
			{
				var (mod, arg) = ContextUserDictionary[post.User.Id];
				await mod.OnRepliedContextually(post, null, arg, Shell, this);
				ContextUserDictionary.Remove(post.User.Id);
				return;
			}

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnDmReceivedAsync(post, Shell, this))
						break;
				}
				catch (Exception ex)
				{
                    await Shell.ReplyAsync(post, "ん...何の話してたんだっけ...?\n\n(エラーが発生したようです。)");
					WriteException(ex);
				}
			}
		}

		public async Task HandleFollowedAsync(IUser user)
		{
			await Task.Delay(400);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnFollowedAsync(user, Shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public static void OpenUrl(string url)
		{
			// from https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
			// hack because of this: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else
			{
				throw new NotSupportedException("このプラットフォームはサポートされていません。");
			}
		}

		public void RegisterContext(IPost post, ModuleBase mod, Dictionary<string, object> args = null)
		{
			if (post is IDirectMessage dm)
			{
				ContextUserDictionary[dm.Recipient.Id] = (mod, args);
			}
			else
			{
				ContextPostDictionary[post.Id] = (mod, args);
			}
		}

		/// <summary>
		/// Resources フォルダ内に配置された組込みリソースを取得します。
		/// </summary>
		/// <param name="path">Resources フォルダからの相対パスを . で繋いだもの。</param>
		/// <returns>取得したリソースのストリーム。</returns>
		public static Stream GetEmbeddedResource(string path)
		{
			var asm = typeof(Server).GetTypeInfo().Assembly;
			return asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.{path}");
		}

		private void WriteException(Exception ex)
		{
			Logger.Error($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public static readonly HttpClient Http = new HttpClient();
		readonly string adminId;
	}
}