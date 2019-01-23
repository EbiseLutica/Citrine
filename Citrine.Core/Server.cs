using System;
using System.Collections.Generic;
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

		readonly string adminName = "Xeltica";
		readonly string adminHost = null;

		private IEnumerable<ModuleBase> modules;

		/// <summary>
		/// 読み込まれているバージョンを列挙します。
		/// </summary>
		/// <value>The modules.</value>
		public IEnumerable<ModuleBase> Modules => Modules;

		/// <summary>
		/// バージョンを取得します。
		/// </summary>
		public static string Version => "2.0.1";

		/// <summary>
		/// XelticaBot 換算でのバージョン表記を取得します。
		/// </summary>
		public static string VersionAsXelticaBot => "3.1.1";

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		public Server()
		{
			modules = Assembly.GetExecutingAssembly().GetTypes()
						.Where(a => a.IsSubclassOf(typeof(ModuleBase)))
						.Select(a => Activator.CreateInstance(a) as ModuleBase)
						.OrderBy(mod => mod.Priority);
			Console.WriteLine($"読み込まれたモジュール({modules.Count()}): {string.Join(", ", modules.Select(mod => mod.GetType().Name))})");
		}

		/// <summary>
		/// 指定したユーザーが管理者であるかどうかを取得します。
		/// </summary>mi
		/// <returns>管理者であれば <c>true</c>、そうでなければ<c>false</c>。</returns>
		/// <param name="user">ユーザー。</param>
		public bool IsAdmin(IUser user) => user.Name?.ToLower() == adminName?.ToLower() && user.Host == adminHost;

		/// <summary>
		/// 指定したユーザーの好感度を取得します。
		/// </summary>
		public Rating GetRatingOf(IUser user) => IsAdmin(user) ? Rating.Partner : Rating.Normal;

		/// <summary>
		/// ユーザーに対する好感度を上げます。
		/// </summary>
		public void Like(IUser user, int amount = 1) { }

		/// <summary>
		/// ユーザーに対する好感度を下げます。
		/// </summary>
		public void Dislike(IUser user, int amount = 1) { Like(user, -amount); }

		private static void WriteException(Exception ex)
		{
			Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}

		public async Task HandleMentionAsync(IPost mention, IShell shell)
		{
			// React
			// hack 好感度システム実装したらそっちに移動して、好感度に応じて love pudding hmm と切り替えていく
			await shell.ReactAsync(mention, IsAdmin(mention.User) ? "❤️" : "");
			foreach (var mod in modules)
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
			foreach (var mod in modules)
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

			foreach (var mod in modules)
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
