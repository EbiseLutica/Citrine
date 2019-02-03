#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Linq;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;
using Disboard.Exceptions;

namespace Citrine.Misskey
{
	/// <summary>
	/// Misskey-specific Module to add Emoji.
	/// </summary>
	public class EmojiModule : ModuleBase
	{
		DateTime lastResetAt = DateTime.Now;
		int registeredCount = 0;
		int limit = 10;
		TimeSpan limitTime = new TimeSpan(0, 5, 0);

		bool IsRateLimitExceeded => registeredCount > limit;

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var text = n.Text.TrimMentions().ToLowerInvariant();
			var cmd = text.Split(' ');
			if (!(shell is Shell s)) return false;
			if (!(shell.Myself is MiUser u)) return false;
			if (!(n is MiPost note)) return false;

			var senderIsAdmin = ((n.User as MiUser).Native.IsAdmin ?? false) || ((n.User as MiUser).Native.IsModerator ?? false) || core.IsAdmin(n.User);

			// /emoji add <name> <url> <alias...>
			// /emoji list
			if (DateTime.Now - lastResetAt > limitTime)
			{
				lastResetAt = DateTime.Now;
				registeredCount = 0; 
			}
			var r = limitTime - (DateTime.Now - lastResetAt);
			string getRemainingTime() => r.Hours > 0 ? r.Minutes + "時間" : r.Minutes > 0 ? r.Minutes + "分" : r.Seconds + "秒";

			if (cmd[0] == "/emoji")
			{
				var output = @"使い方:
/emoji add <name> [url] [alias...]: 絵文字を追加. urlの代わりに添付ファイルも可
/emoji list: ここにある絵文字をぜんぶ並べる
";
				if (senderIsAdmin)
				{
					// 管理者であればヘルプ追加
					output += "/emoji copyfrom <hostname>: 他のインスタンスから絵文字をコピーする";
				}

				string cw = default;
				if ((u.Native.IsAdmin ?? false) || (u.Native.IsModerator ?? false))
				{
					switch (cmd[1].ToLowerInvariant())
					{
						case "add":
							try
							{
								if (IsRateLimitExceeded && !senderIsAdmin)
								{
									output = $"ちょっと追加しすぎ...もう少し待って欲しいな. あと{getRemainingTime()}くらいね.";
								}
								else if (cmd.Length > 2)
								{
									string url = default;
									if (cmd.Length > 3)
									{
										url = cmd[3];
									}
									else if (note.Native.Files?.Count > 0)
									{
										url = note.Native.Files.First().Url;
									}
									await s.Misskey.Admin.Emoji.AddAsync(cmd[2], url, cmd.Skip(4).ToList());
									output = $"追加したよ :{cmd[2]}:";
									registeredCount++;
								}
							}
							catch (Exception ex)
							{
								cw = "失敗しちゃいました... 報告書見ますか?";
								output = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
								Console.WriteLine(ex.Message);
								Console.WriteLine(ex.StackTrace);
							}
							break;
						case "list":
							try
							{
								var list = await s.Misskey.Admin.Emoji.ListAsync();
								output = string.Concat(list.Select(e => $":{e.Name}:"));
								cw = $"絵文字総数: {list.Count}個";
							}
							catch (Exception ex)
							{
								cw = "失敗しちゃいました... 報告書見ますか?";
								output = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
								Console.WriteLine(ex.Message);
								Console.WriteLine(ex.StackTrace);
							}
							break;
						case "copyfrom":
							if (senderIsAdmin)
							{
								if (cmd.Length > 2)
								{
									try
									{
										var emojisInMyHost = await s.Misskey.Admin.Emoji.ListAsync();

										var emojis = (await s.Misskey.Admin.Emoji.ListAsync(cmd[2])).Where(e => emojisInMyHost.Any(ee => ee.Name == e.Name));

										foreach (var emoji in emojis)
										{
											await s.Misskey.Admin.Emoji.AddAsync(emoji.Name, emoji.Url, emoji.Aliases);
										}


										output = string.Concat(emojis.Select(e => $":{e.Name}:"));

										cw = $"{cmd[2]} にある絵文字を {emojis.Count()} 種類追加しました.";
									}
									catch (Exception ex)
									{
										cw = "失敗しちゃいました... 報告書見ますか?";
										output = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
										Console.WriteLine(ex.Message);
										Console.WriteLine(ex.StackTrace);
									}
								}
							}
							else
							{
								output = "それ, 危険すぎるので, 鯖管以外に言われてもやるなと言われてるの."; 
							}
							break;
					}
				}
				else
				{
					output = "僕はここの管理者じゃないから, それはできないんだ...ごめんね"; 
				}
				await shell.ReplyAsync(n, output, cw);
				return true;
			}

			return false;
		}
		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);
	}
}
