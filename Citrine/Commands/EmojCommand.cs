#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Linq;
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
/emoji copyfrom <hostname> (管理者限定)";

		public override string Description => "インスタンスへの絵文字の追加、リストアップ、他インスタンスからのコピーを行います。";

		public override PermissionFlag Permission => PermissionFlag.LocalOnly;

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			var u = (shell.Myself as MiUser).Native;
			var s = shell as Shell;
			var note = (source as MiPost).Native;

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
							(output, cw) = await AddAsync(args, note, u, s);
							break;
						case "list":
							var list = await s.Misskey.Admin.Emoji.ListAsync();
							output = string.Concat(list.Select(e => $":{e.Name}:"));
							cw = $"絵文字総数: {list.Count}個";
							break;
						case "copyfrom":
							(output, cw) = await CopyFromAsync(args, note, u, s);
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
			await shell.ReplyAsync(source, output, cw);
			return null;
		}

        // emoji add <name> <url> [aliases...]
        // emoji add <name> (with a file)
        private async Task<(string, string)> AddAsync(string[] args, Note note, User user, Shell shell)
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
		private async Task<(string, string)> CopyFromAsync(string[] args, Note note, User user, Shell s)
		{
			var isAdmin = note.User.IsAdmin ?? false;
			var isMod = note.User.IsModerator ?? false;
			// 危険だから
			if (!isAdmin && !isMod)
				throw new AdminOnlyException();

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
	}
}
