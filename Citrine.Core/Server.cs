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
		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "5.0.0-dev";

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
				adminId = File.ReadAllText("./admin").Trim().ToLower();
				Console.WriteLine($"管理者はID {adminId ?? "null"}。");
			}
			else
			{
				Console.Write("Admin's ID > ");
				adminId = Console.ReadLine().Trim().ToLower();
				File.WriteAllText("./admin", adminId);
				Console.WriteLine($"管理者はID {adminId ?? "null"}。");
			}

			NicknameMap = new Dictionary<string, string>();
			if (File.Exists("./nicknames"))
			{
				var lines = File.ReadAllLines("./nicknames");
				lines.Select(l => {
					var kv = l.Split(',');
					return new KeyValuePair<string, string>(kv[0], string.Concat(kv.Skip(1)));
				})
				.ForEach(kv => NicknameMap[kv.Key] = kv.Value);
				Console.WriteLine($"Load {lines.Length} user's nickname");
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

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdmin(IUser user) => user.Name.ToLower() == adminId.ToLower() && string.IsNullOrEmpty(user.Host);

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(IUser user) => IsAdmin(user) ? Rating.Partner : Rating.Normal;

		/// <summary>
		/// ユーザーに対する好感度を上げます。
		/// </summary>
		public void Like(string userId, int amount = 1) { }

		/// <summary>
		/// ユーザーに対する好感度を下げます。
		/// </summary>
		public void Dislike(string userId, int amount = 1) { Like(userId, -amount); }

		/// <summary>
		/// ユーザーのニックネームを取得します。
		/// </summary>
		public string GetNicknameOf(IUser user) => NicknameMap.ContainsKey(user.Id) ? NicknameMap[user.Id] : $"{user.Name}さん";

		/// <summary>
		/// ユーザーのニックネームを設定します。
		/// </summary>
		public void SetNicknameOf(IUser user, string name)
		{
			NicknameMap[user.Id] = name;
			SaveNicknames();
		}

		/// <summary>
		/// ユーザーのニックネームを破棄します。
		/// </summary>
		public void ResetNicknameOf(IUser user)
		{
			NicknameMap.Remove(user.Id);
			SaveNicknames();
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
		public Stream GetEmbeddedResource(string path)
		{
			asm = typeof(Server).GetTypeInfo().Assembly;
			return asm.GetManifestResourceStream($"{asm.GetName().Name}.Resources.{path}");
		}

		private void SaveNicknames()
		{
			File.WriteAllLines("./nicknames", NicknameMap.Select(kv => $"{kv.Key},{kv.Value}"));
		}

		private static void WriteException(Exception ex)
		{
			Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public static readonly HttpClient Http = new HttpClient();
		readonly string adminId;
		Assembly asm;
	}
}
