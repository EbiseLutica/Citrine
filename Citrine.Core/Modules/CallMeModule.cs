using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
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
						EconomyModule.Pay(n, shell, core);
						core.LikeWithLimited(n.User);
						await shell.ReplyAsync(n, $"わかった. これからは君のことを{core.GetNicknameOf(n.User)}と呼ぶね.");
						break;
				}
				return true;
			}
			return false;
		}
	}
}
