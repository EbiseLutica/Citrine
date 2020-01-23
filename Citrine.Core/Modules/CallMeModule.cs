using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class SushiModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string text && text.IsMatch("寿司(握|にぎ)"))
			{
				var res = "";
				var s = random.Next(10) > 3 ? null : sushi.Random();
				var max = random.Next(1, 10);
				for (var i = 0; i < max; i++)
					res += s ?? sushi.Random();
				await shell.ReplyAsync(n, "ヘイお待ち! " + res);
				return true;
			}
			return false;
		}

		private readonly Random random = new Random();

		private readonly string[] sushi =
		{
			"🍣", "🍣", "🍣", "🍣", "🍣", "🍣", "🍕", "🍔", "🍱", "🍘", "🍫", "📱", "💻",
		};
	}

	public class CallMeModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;
			var m = Regex.Match(n.Text.TrimMentions(), @"(.+)(って|と)呼[べびん]");
			if (m.Success)
			{
				switch (core.GetRatingOf(n.User))
				{
					case Rating.Hate:
						await shell.ReplyAsync(n, "嫌だ.");
						break;
					case Rating.Normal:
						await shell.ReplyAsync(n, "もう少し仲良くなってからね.");
						break;
					default:
						var nick = m.Groups[1].Value;
						core.SetNicknameOf(n.User, nick);
						await shell.ReplyAsync(n, $"わかった. これからは君のことを{core.GetNicknameOf(n.User)}と呼ぶね.");
						break;
				}
				return true;
			}
			return false;
		}
	}
}
