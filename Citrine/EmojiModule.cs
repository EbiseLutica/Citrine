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

		bool IsRateLimitExceeded => registeredCount > limit;

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var text = n.Text.TrimMentions().ToLowerInvariant();
			var cmd = text.Split(' ');
			if (!(shell is Shell s)) return false;
			if (!(shell.Myself is MiUser u)) return false;
			if (!(n is MiPost note)) return false;

			// /emoji add <name> <url> <alias...>
			// /emoji list
			if (DateTime.Now - lastResetAt > new TimeSpan(0, 5, 0))
			{
				lastResetAt = DateTime.Now;
				registeredCount = 0; 
			}
			if (cmd[0] == "/emoji")
			{
				var output = "使い方:\n/emoji add <name> [url] [alias...]: 絵文字を追加。urlの代わりに添付ファイルも可\n/emoji list: ここにある絵文字をぜんぶ並べる";
				string cw = default;
				if ((u.Native.IsAdmin ?? false) || (u.Native.IsModerator ?? false))
				{
					switch (cmd[1])
					{
						case "add":
							try
							{
								if (cmd.Length > 2)
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
							catch (DisboardException ex)
							{
								output = "失敗しちゃった...";
								Console.WriteLine(ex.Response);
							}
							catch (Exception ex)
							{
								output = $"エラー {ex.GetType().Name} {ex.Message}";
							}
							break;
						case "list":
							try
							{
								var list = await s.Misskey.Admin.Emoji.ListAsync();
								output = string.Concat(list.Select(e => $":{e.Name}:"));
								cw = $"絵文字総数: {list.Count}個";
							}
							catch (DisboardException ex)
							{
								output = "失敗しちゃった...";
								Console.WriteLine(ex.Response);
							}
							catch (Exception ex)
							{
								output = $"エラー {ex.GetType().Name} {ex.Message}";
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
