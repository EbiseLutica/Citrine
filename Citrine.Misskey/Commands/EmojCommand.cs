#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;
using Disboard.Exceptions;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	/// <summary>
	/// Misskey-specific Module to add Emoji.
	/// </summary>
	public class EmojiCommand : CommandBase
	{
		public override string Name => "emoji";

		public override string Usage => @"使い方:
/emoji add <name> [url] [alias...]
/emoji add <name> [url] [alias...]
/emoji list
/emoji copyfrom <hostname> (管理者限定)
/emoji delete <name> (管理者限定)
/emoji delete </regex/> (管理者限定)";

		public override string Description => "インスタンスへの絵文字の追加、リストアップ、他インスタンスからのコピーを行います。";

		public override PermissionFlag Permission => PermissionFlag.LocalOnly;

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (!(sender is PostCommandSender p))
				return "このコマンドはユーザーが実行してください.";
			var u = (shell.Myself as MiUser).Native;
			var s = shell as Shell;
			var note = (p.Post as MiPost).Native;

			if (u == null || note == null || s == null)
			{
				return "ここは, Misskey ではないようです. このコマンドがここにあるのは有り得ない状況なので, おそらくバグかな.";
			}

			if (args.Length < 1)
			{
				throw new CommandException();
			}
			string output, cw;
			if ((u.IsAdmin ?? false) || (u.IsModerator ?? false))
			{
				try
				{
					switch (args[0])
					{
						case "add":
							(output, cw) = await AddAsync(args, note, s);
							break;
						case "list":
							var list = await s.Misskey.Admin.Emoji.ListAsync();
							output = string.Concat(list.Select(e => $":{e.Name}:"));
							cw = $"絵文字総数: {list.Count}個";
							break;
						case "copyfrom":
							if (!sender.IsAdmin)
								throw new AdminOnlyException();
							(output, cw) = await CopyFromAsync(args, s);
							break;
						case "delete":
							if (!sender.IsAdmin)
								throw new AdminOnlyException();
							(output, cw) = await DeleteAsync(args, s, p.Post);
							break;
						default:
							throw new CommandException();
					}
				}
				catch (Exception ex) when (!((ex is CommandException) || (ex is AdminOnlyException)))
				{
					cw = "失敗しちゃいました... 報告書見ますか?";
					output = $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.StackTrace);
				}
			}
			else
			{
				return "僕はここの管理者じゃないから, それはできないんだ...ごめんね";
			}
			await shell.ReplyAsync(p.Post, output, cw);
			return null;
		}

		// emoji add <name> <url> [aliases...]
		// emoji add <name> (with a file)
		private async Task<(string, string)> AddAsync(string[] args, Note note, Shell shell)
		{
			if (args.Length < 2)
				throw new CommandException();
			string url = default;
			if (args.Length >= 3)
			{
				url = args[2];
			}
			else if (note.Files?.Count > 0)
			{
				url = note.Files.First().Url;
			}
			await shell.Misskey.Admin.Emoji.AddAsync(args[1], url, args.Skip(note.Files?.Count > 0 ? 2 : 3).ToList());
			return ($"追加したよ :{args[1]}:", null);
		}

		// /emoji copyfrom <host>
		private async Task<(string, string)> CopyFromAsync(string[] args, Shell s)
		{
			if (args.Length < 2)
				throw new CommandException();

			var host = args[1];

			var emojisInMyHost = await s.Misskey.Admin.Emoji.ListAsync();

			var emojis = (await s.Misskey.Admin.Emoji.ListAsync(host))
				.Where(e => emojisInMyHost.All(ee => ee.Name != e.Name));

			await emojis.ForEach(emoji => s.Misskey.Admin.Emoji.AddAsync(emoji.Name, emoji.Url, emoji.Aliases));

			var count = emojis.Count();

			if (count > 0)
			{
				return (string.Concat(emojis.Select(e => $":{e.Name}:")),
						$"{host} にある絵文字を {count} 種類追加しました.");
			}
			else
			{
				return ("何も追加できませんでした.", null);
			}
		}


		// /emoji delete <name>
		// /emoji delete </regexp/>
		private async Task<(string, string)> DeleteAsync(string[] args, Shell s, IPost p)
		{
			int interval = 60;
			string GetTime(int cnt)
			{
				double time = cnt * interval / 1000f;
				string suffix = "秒";
				if (time > 59)
				{
					time /= 60;
					suffix = "分";
				}
				if (time > 59)
				{
					time /= 60;
					suffix = "時間";
				}
				if (time > 23)
				{
					time /= 24;
					suffix = "日";
				}
				return time < 1 ? "一瞬" : time + suffix;

			}
			if (args.Length < 2)
				throw new CommandException();
			var isRegexMode = args[1].StartsWith('/') && args[1].EndsWith('/');
			Func<Emoji, bool> expr;

			if (isRegexMode)
			{
				var r = new Regex(args[1].Remove(0, 1).Remove(args[1].Length - 2));
				expr = (e) => r.IsMatch(e.Name);
			}
			else
			{
				expr = (e) => e.Name == args[1];
			}

			var list = await s.Misskey.Admin.Emoji.ListAsync();
			var matches = list.Where(expr);
			var count = matches.Count();

			var note = await s.ReplyAsync(p, $"{count} 件の絵文字を削除します. この作業は{GetTime(count)}で終わると推測されます. ");

			await matches.ForEach(e => Task.WhenAll(Task.Delay(interval), s.Misskey.Admin.Emoji.RemoveAsync(e.Id)));
			await s.Misskey.Notes.DeleteAsync(note.Id);
			return ("指定された絵文字を削除しました。", "");
		}
	}
}
