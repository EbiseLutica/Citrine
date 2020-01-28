using System;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class ImHereModule : ModuleBase
	{
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
				await shell.ReplyAsync(n, patterns.Random(rnd));
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
