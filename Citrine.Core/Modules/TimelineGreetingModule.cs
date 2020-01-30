using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class TimelineGreetingModule : ModuleBase
	{
		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (!(n.Text?.TrimMentions() is string text))
				return false;
			if (core.GetRatingOf(n.User) < Rating.Like)
				return false;

			var storage = core.Storage[n.User];

			// 返事する
			foreach (var (greeting, pattern, replyPattern) in greetings)
			{
				if (pattern.IsMatch(text) && !IsAlreadyDone(greeting, n.User, core))
				{
					UpdateGreeting(greeting, n.User, core);
					await Task.Delay(2000);
					await shell.ReplyAsync(n, replyPattern.Random().Replace("$user$", core.GetNicknameOf(n.User)));
					break;
				}
			}

			// おやすみを聞いてからしばらく発言してるユーザーにツッコむ
			// おやすみから6時間経過していれば気にしない
			var timeSpan = DateTime.Now - storage.Get(KeyOf(Greeting.GoodNight), DateTime.MinValue);
			if (!patternGoodNight.IsMatch(text) && timeSpan <= new TimeSpan(6, 0, 0))
			{
				var count = storage.Get(postWithoutSleepingCountKey, 0);
				storage.Set(postWithoutSleepingCountKey, ++count);
				if (count == 3)
				{
					await shell.ReplyAsync(n, postWithoutSleepingReply.Random().Replace("$user$", core.GetNicknameOf(n.User)));
				}
			}

			return false;
		}

		public bool IsAlreadyDone(Greeting greeting, IUser user, Server core)
		{
			var storage = core.Storage[user];
			var lastGreeted = storage.Get(KeyOf(greeting), DateTime.MinValue);
			return DateTime.Now.Date == lastGreeted.Date;
		}

		public void UpdateGreeting(Greeting greeting, IUser user, Server core)
		{
			var storage = core.Storage[user];
			storage.Set(KeyOf(greeting), DateTime.Now);
		}

		public string KeyOf(Greeting greeting) => $"last-greeted-datetime.{greeting}";

		private readonly string postWithoutSleepingCountKey = $"post-without-sleeping-count";

		private readonly (Greeting greeting, Regex regex, string[] replyPattern)[] greetings =
		{
			(Greeting.GoodMorning, patternGoodMorning, new []
			{
				"おはよ〜!",
				"おはよ, $user$.",
			}),
			(Greeting.GoodNight, patternGoodNight, new []
			{
				"おやすみなさい",
				"おやすみ, $user$.",
				"おやすみ",
				"ちゃんと寝るんだぞ〜$user$."
			}),
			(Greeting.SeeYouLater, patternSeeYouLater, new []
			{
				"いってらっしゃい!",
				"いってら!",
				"頑張ってね〜!",
				"いってらっしゃい, $user$.",
			}),
			(Greeting.WelcomeBack, patternWelcomeBack, new []
			{
				"おかえり",
				"おつかれ〜!",
				"おかえり, $user$!",
				"おかえりなさい, $user$.",
			}),
		};

		private readonly string[] postWithoutSleepingReply =
		{
			"寝るんじゃないの〜?",
			"おやすみじゃなかったのかい, $user$.",
			"寝る寝る詐欺はよくないよ〜",
			"おやすみって言ってたのにいつまで投稿してんの〜!早く寝なさい!",
			"$user$, おやすみじゃなかったのか〜?"
		};

		public enum Greeting
		{
			GoodMorning,
			GoodNight,
			SeeYouLater,
			WelcomeBack,
		}

		private static readonly Regex patternGoodMorning = new Regex("^おはよ$|おはよ[うおー〜]");
		private static readonly Regex patternGoodNight = new Regex("おやすみ|[寝ね](ます|る)");
		private static readonly Regex patternSeeYouLater = new Regex("[い行]って(きます|くる)");
		private static readonly Regex patternWelcomeBack = new Regex("ただいま");
	}
}
