using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class TimelineGreetingModule : ModuleBase
	{
		public static readonly string StatGoodMorningCount = "stat.good-morning-count";
		public static readonly string StatGoodNightCount = "stat.good-night-count";
		public static readonly string StatSeeYouLaterCount = "stat.see-you-later-count";
		public static readonly string StatWelcomeBackCount = "stat.welcome-back-count";
		public static readonly string StatWithoutSleepingCount = "stat.without-sleeping-count";

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text?.TrimMentions() is not string text)
				return false;
			if (core.GetRatingOf(n.User) < Rating.Like)
				return false;
			// リプライであれば邪魔しない
			if (n.Reply != null)
				return false;

			var storage = core.Storage[n.User];

			// 返事する
			foreach (var (greeting, pattern, length, replyPattern, statKey) in greetings)
			{
				if (text.Length <= length && pattern.IsMatch(text) && !IsAlreadyDone(greeting, n.User, core))
				{
					UpdateGreeting(greeting, n.User, core);
					storage.Set(postWithoutSleepingCountKey, 0);
					await Task.Delay(2000);
					core.Storage[n.User].Add(statKey);
					var ctx = await shell.ReplyAsync(n, replyPattern.Random().Replace("$user$", core.GetNicknameOf(n.User)));
					if (ctx != null)
	                    core.RegisterContext(ctx, this);
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
					core.Storage[n.User].Add(StatWithoutSleepingCount);
				}
			}

			return false;
		}

        public override async Task<bool> OnRepliedContextually(IPost n, IPost? context, Dictionary<string, object> store, IShell shell, Server core)
        {
			if (n.Text == null) return false;

			var storage = core.Storage[n.User];

            if (n.Text.IsMatch("(ちが|違)う|じゃない|([寝ね](ない|にゃい|ね|ません))") && storage.Has("last-greeted-datetime.cache"))
            {
                await shell.ReplyAsync(n, "あれ, 勘違いだった? ごめんね. 忘れておく");
                storage.Set(KeyOf(Greeting.GoodNight), storage.Get("last-greeted-datetime.cache", DateTime.MinValue));
                storage.Clear("last-greeted-datetime.cache");
                return true;
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

            if (greeting == Greeting.GoodNight)
                storage.Set("last-greeted-datetime.cache", storage.Get(KeyOf(greeting), DateTime.MinValue));

            storage.Set(KeyOf(greeting), DateTime.Now);
		}

		public string KeyOf(Greeting greeting) => $"last-greeted-datetime.{greeting}";

		private readonly string postWithoutSleepingCountKey = $"post-without-sleeping-count";

		private readonly (Greeting greeting, Regex regex, int maxLength, string[] replyPattern, string statKey)[] greetings =
		{
			(Greeting.GoodMorning, patternGoodMorning, 11, new []
			{
				"おはよー",
				"おはよ, $user$.",
			}, StatGoodMorningCount),
			(Greeting.GoodNight, patternGoodNight, 9, new []
			{
				"おやすみなさい",
				"おやすみ, $user$.",
				"おやすみ",
				"ちゃんと寝るんだぞー$user$."
			}, StatGoodNightCount),
			(Greeting.SeeYouLater, patternSeeYouLater, 9, new []
			{
				"いってらっしゃい",
				"いってら",
				"頑張ってねー",
				"いってらっしゃい, $user$.",
			}, StatSeeYouLaterCount),
			(Greeting.WelcomeBack, patternWelcomeBack, 12, new []
			{
				"おかえり",
				"おつかれー",
				"おかえり, $user$",
				"おかえりなさい, $user$.",
			}, StatWelcomeBackCount),
		};

		private readonly string[] postWithoutSleepingReply =
		{
			"寝るんじゃないの?",
			"おやすみじゃなかったのかい, $user$.",
			"寝る寝る詐欺はよくないよ",
			"おやすみって言ってたのにいつまで投稿してんのー?",
			"$user$, おやすみじゃなかったの"
		};

		public enum Greeting
		{
			GoodMorning,
			GoodNight,
			SeeYouLater,
			WelcomeBack,
		}

		private static readonly Regex patternGoodMorning = new Regex("^おはよ$|おはよ[うおーー]");
		private static readonly Regex patternGoodNight = new Regex("おやすみ|[寝ね](ます|る)");
		private static readonly Regex patternSeeYouLater = new Regex("[い行]って(きます|くる)");
		private static readonly Regex patternWelcomeBack = new Regex("ただいま");
	}
}
