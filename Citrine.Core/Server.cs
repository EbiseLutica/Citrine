using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
		// hack 止めよう、ハードコーディング
		readonly string adminId;

		/// <summary>
		/// 読み込まれているバージョンを列挙します。
		/// </summary>
		/// <value>The modules.</value>
		public IEnumerable<ModuleBase> Modules { get; }

		List<ModuleBase> ModulesAsList => Modules as List<ModuleBase>;

		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "2.3.0";

		/// <summary>
		/// XelticaBot 換算でのバージョン表記を取得します。
		/// </summary>
		public static string VersionAsXelticaBot => "3.4.0";

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		public Server(params ModuleBase[] additionalModules)
		{
			Modules = Assembly.GetExecutingAssembly().GetTypes()
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


			Console.WriteLine($"読み込まれたモジュール({Modules.Count()}): {string.Join(", ", Modules.Select(mod => mod.GetType().Name))})");
		}

		public void AddModule(ModuleBase mod) => ModulesAsList?.Add(mod);

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdmin(IUser user) => IsAdmin(user.Id);

		public bool IsAdmin(string userId) => userId.ToLower() == adminId.ToLower();

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(IUser user) => GetRatingOf(user.Id);

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(string userId) => IsAdmin(userId) ? Rating.Partner : Rating.Normal;

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public int GetRatingNumber(string userId) => IsAdmin(userId) ? 100 : 0;

		/// <summary>
		/// ユーザーに対する好感度を上げます。
		/// </summary>
		public void Like(string userId, int amount = 1) { }

		/// <summary>
		/// ユーザーに対する好感度を下げます。
		/// </summary>
		public void Dislike(string userId, int amount = 1) { Like(userId, -amount); }

		private static void WriteException(Exception ex)
		{
			Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public async Task HandleMentionAsync(IPost mention, IShell shell)
		{
			// React
			// hack 好感度システム実装したらそっちに移動して、好感度に応じて love pudding hmm と切り替えていく
			Console.WriteLine($"Mentioned from @{mention.User.Name}");
			await shell.ReactAsync(mention, IsAdmin(mention.User) ? "❤️" : "🍣");
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
			Console.WriteLine($"Mentioned from @{post.User.Name}");
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
