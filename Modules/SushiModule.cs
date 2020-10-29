using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class SushiModule : ModuleBase
	{
		public static readonly string StatSushiCount = "stat.sushi-count";

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string)
            {
                var m = Regex.Match(n.Text.TrimMentions(), "^(.+)(握|にぎ)");
                if (m.Success)
                {
                    var target = m.Groups[1].Value;
                    if (target.Contains("寿司") | target.Contains("すし"))
                    {
                        var candidates = core.GetRatingOf(n.User) == Rating.Hate ? dusts : sushi;
                        var res = "";
                        var s = random.Next(10) > 3 ? null : candidates.Random();
                        var max = random.Next(1, 10);
                        for (var i = 0; i < max; i++)
                            res += s ?? candidates.Random();
                        await shell.ReplyAsync(n, "ヘイお待ち! " + res);
                        core.Storage[n.User].Add(StatSushiCount);
                        EconomyModule.Pay(n, shell, core);
                        core.LikeWithLimited(n.User);
                    }
                    else if (target.Length < 5)
                    {
                        await shell.ReplyAsync(n, messagesNigiri.Random().Replace("$user$", core.GetNicknameOf(n.User)).Replace("$thing$", target));
                        EconomyModule.Pay(n, shell, core);
                        core.LikeWithLimited(n.User);
                    }
                    else
                    {
                        await shell.ReplyAsync(n, messagesReject.Random().Replace("$user$", core.GetNicknameOf(n.User)).Replace("$thing$", target));
                        EconomyModule.Pay(n, shell, core);
                        core.LikeWithLimited(n.User);
                    }
                    return true;
                }
            }
            return false;
		}

		private readonly Random random = new Random();

		private readonly string[] sushi =
		{
			"🍣", "🍣", "🍣", "🍣", "🍣", "🍣", "🍣", "🍣", "🍕", "🍔", "🍱", "🍘", "🍫", "📱", "💻",
		};

		private readonly string[] dusts =
		{
			"🐛", "🍂", "🥦", "💩"
		};

        private readonly string[] messagesNigiri =
        {
			"$user$の$thing$, 握ったよ",
            "$user$の$thing$を握りました",
            "$user$の$thing$は私の物です🥴",
        };

        private readonly string[] messagesReject =
        {
            "それはちょっと...",
			"それはさすがに無理かな",
			"ちょっとそれは厳しい",
			"厳しい",
			"無理です",
			"握れません",
			"$user$の$thing$は握るには難しい"
        };

    }
}
