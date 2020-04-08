using System;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class ImHereModule : ModuleBase
	{
		public static readonly string StatImHereCount = "stat.im-here-count";
		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text != null && n.Text.TrimMentions().ToHiragana().IsMatch(@"^しとりん(ちゃん|さん|様)?(何処|どこ|[居い](ますか|る[\?？]))"))
			{
				// 友好度が低ければやらない
				if (core.GetRatingOf(n.User) < Rating.Like)
					return false;

				// 遊び時間
				await Task.Delay(3000 + rnd.Next(4000));
				await shell.ReactAsync(n, "❤️");

				await Task.Delay(250);
				core.Storage[n.User].Add(StatImHereCount);
				await shell.ReplyAsync(n, patterns.Random(rnd).Replace("{user}", core.GetNicknameOf(n.User)));
			}
			return false;
		}
		private readonly Random rnd = new Random();

		private readonly string[] patterns =
		{
			"ここだよ〜.",
			"ここにいるよ!",
			"ここ!",
			"どうしたの〜?",
			"どうしたの〜{user}.",
			"{user}〜! ここにいるよ!",
			"いないよ〜(うそ)",
		};
	}
}
