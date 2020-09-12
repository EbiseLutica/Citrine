using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	/* === リプライ文字列の仕様 ===
	 * $user$ は相手のユーザー名, もしくはニックネームに置き換わる
	 * $prefix$ はラッキーアイテムの修飾子辞書からランダムに取る
	 * $item$ はラッキーアイテム辞書からランダムに取る
	 * $rndA,B$はAからBまでの乱数
	 */
	public class GreetingModule : ModuleBase
	{
		public override int Priority => 10000;
		readonly List<Pattern> patterns;
		readonly Random random = new Random();
		public static readonly string StatTalkedCount = "stat.talked-count";

		public GreetingModule()
		{
			using var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("Citrine.Resources.greeting.json"));
			patterns = JsonConvert.DeserializeObject<List<Pattern>>(reader.ReadToEnd());
		}

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;

			core.LikeWithLimited(n.User);

			var pattern = patterns.FirstOrDefault(record => Regex.IsMatch(n.Text.Trim().Replace("にゃ", "な"), record.Regex));

			if (pattern == null)
				return false;

			await Task.Delay(2000 + random.Next(7000));

			var message = (core.GetRatingOf(n.User)) switch
			{
				Rating.Hate => pattern.Hate(),
				Rating.Normal => pattern.Normal(),
				Rating.Like => pattern.Like(),
				Rating.BestFriend => pattern.BestFriend(),
				Rating.Partner => pattern.Partner(),
				_ => "",
			};

			message = message
						.Replace("$user$", core.GetNicknameOf(n.User))
						.Replace("$prefix$", FortuneModule.ItemPrefixes.Random())
						.Replace("$item$", FortuneModule.Items.Random());

			// 乱数
			message = Regex.Replace(message, @"\$rnd(\d+),(\d+)\$", (m) =>
			{
				return random.Next(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)).ToString();
			});

			// からっぽは既読無視
			if (message != "")
			{
				EconomyModule.Pay(n, shell, core);
				core.Storage[n.User].Add(StatTalkedCount);
				await shell.ReplyAsync(n, message);
			}

			return true;
		}


		public class Pattern
		{
			[JsonProperty("regex")]
			public string Regex { get; set; } = "";

			[JsonProperty("replyNormal")]
			public string[] ReplyNormal { get; set; } = new string[0];

			[JsonProperty("replyPartner")]
			public string[] ReplyPartner { get; set; } = new string[0];

			[JsonProperty("replyHate")]
			public string[] ReplyHate { get; set; } = new string[0];

			[JsonProperty("replyBestFriend")]
			public string[] ReplyBestFriend { get; set; } = new string[0];

			[JsonProperty("replyLike")]
			public string[] ReplyLike { get; set; } = new string[0];
		}
	}

	public static class PatternExtension
	{
		public static string Hate(this GreetingModule.Pattern p)
			=> p.ReplyHate?.Random() ?? p.Normal();

		public static string Normal(this GreetingModule.Pattern p)
			=> p.ReplyNormal?.Random() ?? "null";

		public static string Like(this GreetingModule.Pattern p)
			=> p.ReplyLike?.Random() ?? p.Normal();

		public static string BestFriend(this GreetingModule.Pattern p)
			=> p.ReplyBestFriend?.Random() ?? p.Like();

		public static string Partner(this GreetingModule.Pattern p)
			=> p.ReplyPartner?.Random() ?? p.BestFriend();
	}

}
