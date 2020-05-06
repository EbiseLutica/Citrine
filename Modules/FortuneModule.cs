using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	using static FortuneModule;
	using static FortuneExtension;

	public partial class FortuneModule : ModuleBase
	{
		public static readonly string StatFortuneCount = "stat.fortune-count";

		public async override Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text != null && Regex.IsMatch(n.Text.ToLowerInvariant(), "占|運勢|みくじ|fortune"))
			{
				core.LikeWithLimited(n.User);
				var isPremium = EconomyModule.HasItem(n.User, "fortuneplus", core);
				var r = new Random(n.User.Id.GetHashCode() + DateTime.Now.Day + DateTime.Now.Month - DateTime.Now.Year);

				int min = 1;
				int max = 6;

				int love = r.Next(min, max),
					money = r.Next(min, max),
					work = r.Next(min, max),
					study = r.Next(min, max);

				// premium
				int health = r.Next(min, max),
					hobby = r.Next(min, max),
					sns = r.Next(min, max),
					gaming = r.Next(min, max),
					meal = r.Next(min, max),
					goingOut = r.Next(min, max),
					shopping = r.Next(min, max);

				var builder = new StringBuilder();

				var list = new List<(string name, string emoji, int value, string bestMessage, string worstMessage)>
				{
					("恋愛運", "❤", love, "気になるあの人にアタックしてみては...?", "グイグイ迫るとかえって痛い目を見るかも..."),
					("金運", "💰", money, "意外なことで得するかも...", "ぼったくりには気をつけてね."),
					("仕事運", "💻", work, "日頃の頑張りがきっと報われるよ.", "やる気が空回りして大ミスしちゃわないように気をつけてね."),
					("勉強運", "📒", study, "昨日わからなかったことがわかる日かも.", "無理して勉強しても頭に入らないかも...")
				};

				if (isPremium)
				{
					list.Add(("健康運", "💪", health, "今日一日バリバリ過ごせることでしょう.", "風邪を引かないよう気をつけて..."));
					list.Add(("趣味運", "🎸", hobby, "好きなことに打ち込もう!", "挫折に気をつけてね."));
					list.Add(("食事運", "🍣", meal, "奮発して美味しい出前を取ろう.", "まずい食べ物に巡り合っちゃうかも..."));
					list.Add(("SNS運", "💬", sns, "喧嘩なく, 平和に過ごせるよ.", "何気ない発言で炎上しちゃうかも...気をつけて."));
					list.Add(("ゲーム運", "🎮", gaming, "スコアが伸びる日だよ. やり込もう!", "頑張ってもスコアが伸びない日だよ. 無理はしないで."));
					list.Add(("ステイホーム運", "🏠", goingOut, "のんびりお家で好きなことをして過ごそう.", "することに飽きてしまってませんか? 新たな趣味を見つける旅をしてみよう!"));
					list.Add(("買いもの運", "👜", shopping, "思い切って欲しかったものを買ってみては?", "不良品を引いちゃうかも. お買い物は程々に."));
				}

				var avg = (int)Math.Round(list.Average(el => el.value));

				builder.AppendLine($"***{results[avg - 1]}***");

				list.ForEach(r => builder.AppendLine($"{r.name}{r.emoji}: {GetStar(r.value, 5)}"));

				var luckyItem = GenerateWord(r);

				builder.AppendLine($"ラッキーアイテム💎: {luckyItem}");

				if (isPremium)
				{
					var orderby = list.OrderByDescending(r => r.value);
					var best = orderby.First();
					var worst = orderby.Last();
					builder
						.AppendLine()
						.Append("シトリンから一言: ")
						.Append($"{worst.name}が低いね. {worst.worstMessage}")
						.AppendLine($"{best.name}が高いね. {best.bestMessage}");
				}

				core.Storage[n.User].Add(StatFortuneCount);
				await shell.ReplyAsync(n, builder.ToString(), $"私が今日の{core.GetNicknameOf(n.User)}の運勢を占ったよ:");
				EconomyModule.Pay(n, shell, core);
				return true;
			}

			return false;
		}

		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);

		static string GetStar(int value, int maxValue) => new string('★', value) + new string('☆', maxValue - value);

	}

	static class FortuneExtension
	{
		public static string GenerateWord(Random? r = null)
		{
			var sb = new StringBuilder();
			var p = ItemPrefix(r);
			var i = Item(r);
			var s = ItemSuffix(r);
			if ((r ?? rnd).Next(100) > 50)
				sb.Append(p);
			sb.Append(i);
			if ((r ?? rnd).Next(100) > 70)
				sb.Append(s);
			return sb.ToString();
		}

		public static string Item(Random? r = null) => Items.Random(r);
		public static string ItemPrefix(Random? r = null) => ItemPrefixes.Random(r);
		public static string ItemSuffix(Random? r = null) => ItemSuffixes.Random(r);

		static readonly Random rnd = new Random();
	}

}
