using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
		/// 読み込まれているバージョンを列挙します。
		/// </summary>
		/// <value>The modules.</value>
		public IEnumerable<ModuleBase> Modules { get; }

		public List<ModuleBase> ModulesAsList => Modules as List<ModuleBase>;

		public Dictionary<string, string> NicknameMap { get; }

		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "2.8.0";

		/// <summary>
		/// XelticaBot 換算でのバージョン表記を取得します。
		/// </summary>
		public static string VersionAsXelticaBot => "3.8.0";

        static Server()
        {
			Http.DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 Citrine/{Server.Version} XelticaBot/{Server.VersionAsXelticaBot} (https://github.com/xeltica/citrine) .NET/{Environment.Version}");
        }

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		public Server(params ModuleBase[] additionalModules)
		{
			AllLoadedModules = Assembly.GetExecutingAssembly().GetTypes()
						.Where(a => a.IsSubclassOf(typeof(ModuleBase)))
						.Select(a => Activator.CreateInstance(a) as ModuleBase)
						.Concat(additionalModules)
						.OrderBy(mod => mod.Priority)
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

			unloadedModules = File.Exists("./unloaded") ? File.ReadAllLines("./unloaded").ToList() : new List<string>();

			// unloaded でないかどうか
			Modules = AllLoadedModules.Where(m => unloadedModules.All(um => um.ToLower() != m.GetType().Name.ToLower()));

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

			Console.WriteLine($"読み込まれたモジュール({Modules.Count()}): {string.Join(", ", Modules.Select(mod => mod.GetType().Name))})");
		}

		public void AddModule(ModuleBase mod) => ModulesAsList?.Add(mod);

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdmin(IUser user) => user.Id.ToLower() == adminId.ToLower() && string.IsNullOrEmpty(user.Host);

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

		private void SaveNicknames()
		{
			File.WriteAllLines("./nicknames", NicknameMap.Select(kv => $"{kv.Key},{kv.Value}"));
		}

		/// <summary>
		/// モジュールをアンロードします。
		/// </summary>
		public void Unload(string name)
		{
			unloadedModules.Add(name);
			ModulesAsList.RemoveAll(m => m.GetType().Name.ToLower() == name);
			WriteUnloadedConfig();
		}

		/// <summary>
		/// モジュールをロードします。
		/// </summary>
		public void Load(string name)
		{
			unloadedModules.Add(name);
			ModulesAsList.RemoveAll(m => m.GetType().Name.ToLower() == name);
			WriteUnloadedConfig();
		}

		private void WriteUnloadedConfig()
		{
			File.WriteAllLines("./unloaded", unloadedModules);
		}

		private void ReadUnloadedConfig()
		{
			unloadedModules = File.Exists("./unloaded") ? File.ReadAllLines("./unloaded").ToList() : new List<string>();
		}

		private static void WriteException(Exception ex)
		{
			Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public async Task HandleMentionAsync(IPost mention, IShell shell)
		{
			// React
			// hack 好感度システム実装したらそっちに移動して、好感度に応じて love pudding hmm と切り替えていく
			await Task.Delay(1000);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.ActivateAsync(mention, shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public async Task HandleTimelineAsync(IPost post, IShell shell)
		{
			await Task.Delay(1000);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnTimelineAsync(post, shell, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		public async Task HandleDmAsync(IPost post, IShell shell)
		{
			await Task.Delay(1000);

			// 非同期実行中にモジュール追加されると例外が発生するので毎回リストをクローン
			foreach (var mod in Modules.ToList())
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnDmReceivedAsync(post, shell, this))
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

		public static readonly HttpClient Http = new HttpClient();
		List<ModuleBase> AllLoadedModules;
		List<string> unloadedModules;
		readonly string adminId;
	}

	public enum Rating
	{
		/// <summary>
		/// 嫌い
		/// </summary>
		Hate,
		/// <summary>
		/// 普通
		/// </summary>
		Normal,
		/// <summary>
		/// 友達
		/// </summary>
		Like,
		/// <summary>
		/// 親友
		/// </summary>
		BestFriend,
		/// <summary>
		/// ご主人様
		/// </summary>
		Partner,
	}

}
