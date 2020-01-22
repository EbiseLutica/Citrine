using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;
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
		List<Pattern> patterns;
		readonly Random random = new Random();
		public GreetingModule()
		{
			using (var reader = new StreamReader(Server.GetEmbeddedResource("greeting.json")))
			{
				patterns = JsonConvert.DeserializeObject<List<Pattern>>(reader.ReadToEnd());
			}
		}

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;

			core.LikeWithLimited(n.User);

			var pattern = patterns.FirstOrDefault(record => Regex.IsMatch(n.Text.Trim().Replace("にゃ", "な"), record.Regex));

			if (pattern == null)
				return false;

			string message;

			switch (core.GetRatingOf(n.User))
			{
				case Rating.Hate:
					message = pattern.Hate();
					break;
				case Rating.Normal:
					message = pattern.Normal();
					break;
				case Rating.Like:
					message = pattern.Like();
					break;
				case Rating.BestFriend:
					message = pattern.BestFriend();
					break;
				case Rating.Partner:
					message = pattern.Partner();
					break;
				default:
					message = "...?";
					break;
			}

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
				await shell.ReplyAsync(n, message);

			return true;
		}


		public class Pattern
		{
			[JsonProperty("regex")]
			public string Regex { get; set; }

			[JsonProperty("replyNormal")]
			public string[] ReplyNormal { get; set; }

			[JsonProperty("replyPartner")]
			public string[] ReplyPartner { get; set; }

			[JsonProperty("replyHate")]
			public string[] ReplyHate { get; set; }

			[JsonProperty("replyBestFriend")]
			public string[] ReplyBestFriend { get; set; }

			[JsonProperty("replyLike")]
			public string[] ReplyLike { get; set; }
		}
	}

	public static class PatternExtension
	{
		public static string Hate(this GreetingModule.Pattern p)
		{
			return p.ReplyHate?.Random() ?? p.Normal();
		}
		public static string Normal(this GreetingModule.Pattern p)
		{
			return p.ReplyNormal?.Random() ?? "null";
		}
		public static string Like(this GreetingModule.Pattern p)
		{
			return p.ReplyLike?.Random() ?? p.Normal();
		}
		public static string BestFriend(this GreetingModule.Pattern p)
		{
			return p.ReplyBestFriend?.Random() ?? p.Like();
		}
		public static string Partner(this GreetingModule.Pattern p)
		{
			return p.ReplyPartner?.Random() ?? p.BestFriend();
		}
	}

}
