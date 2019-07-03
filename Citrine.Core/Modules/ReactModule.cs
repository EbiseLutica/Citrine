using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class ReactModule : ModuleBase
	{
		public override int Priority => -1000;

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself.Id)
				return false;

			if (n.Text == null)
				return false;

			var m = Regex.Match(n.Text, "((?:\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]|[\\:a-z0-9A-Z_]+))を?[投な]げ[てろよ]");
			if (m.Success)
			{
				await shell.ReactAsync(n, m.Groups[1].Value.Trim());
			}
			else if (n.Text.IsMatch("ぽんこつ|ポンコツ"))
			{
				await shell.ReactAsync(n, "💢");
			}

			// 多分競合しないから常にfalse
			return false;
		}

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (string.IsNullOrEmpty(n.Text))
				return false;

			// しかと
			if (core.GetRatingOf(n.User) == Rating.Hate)
				return false;

			var murakamiStyleReturnMethod = Regex.Match(n.Text, "帰宅しよ[うっ]?かな?");
			if (murakamiStyleReturnMethod.Success)
			{
				await shell.ReactAsync(n, "😮");
				return true;
			}
			
			var tukareta = Regex.Match(n.Text, "帰宅|帰っ(てき)?た|[お終]わっ?た|(しご|がこ|ば)おわ|(疲|つか)れた");
			if (tukareta.Success)
			{
				await shell.ReactAsync(n, "🎉");
				if (rnd.Next(100) < 20)
				{
                    await shell.ReplyAsync(n, otsukarePattern.Random());
					return true;
				}
			}

			var morning = Regex.Match(n.Text, "起床|[起おぽ]きた|起きました|おはよう");
			if (morning.Success)
			{
				await shell.ReactAsync(n, "🎉");
				if (rnd.Next(100) < 20)
				{
                    await shell.ReplyAsync(n, ohayouPattern.Random());
					return true;
				}
			}

			var sleep = Regex.Match(n.Text, "寝ます|寝る|ねる|[ぽお]や[しす]み");
			if (morning.Success)
			{
				await shell.ReactAsync(n, "👍");
				if (rnd.Next(100) < 20)
				{
                    await shell.ReplyAsync(n, oyasumiPattern.Random());
					return true;
				}
			}

			var ittera = Regex.Match(n.Text, "行って(き|まいり|参り)ます|行ってくる");
			if (ittera.Success)
			{
				await shell.ReactAsync(n, "👍");
				if (rnd.Next(100) < 20)
				{
                    await shell.ReplyAsync(n, itteraPattern.Random());
					return true;
				}
			}
			
			return false;
		}
		private static readonly Random rnd = new Random();
		private static readonly string[] otsukarePattern = 
		{
			"おつかれ〜!",
			"おつかれ.",
			"お疲れ様です",
			"おつです",
			"今日も一日お疲れ様でした.",
		};

		private static readonly string[] ohayouPattern = 
		{
			"おはよ〜!",
			"おはよ!",
			"おはようございます!",
			"おはよう",
		};

		private static readonly string[] oyasumiPattern = 
		{
			"おやすみ!",
			"おやすみ〜!",
			"良い夢を!",
			"おやすみなさい!",
		};

		private static readonly string[] itteraPattern = 
		{
			"いってらっしゃいませ!",
			"いってら!",
			"いってらっしゃい!",
			"いってら〜!",
		};
	}
}
