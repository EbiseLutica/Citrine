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

			var arkStyleReturnMethod = Regex.Match(n.Text, "帰宅しよ[うっ]?かな?");
			var returned = Regex.Match(n.Text, "帰宅|帰っ(てき)?た|[お終]わっ?た|(しご|がこ|ば)おわ|(疲|つか)れた");
			if (arkStyleReturnMethod.Success)
			{
				await shell.ReactAsync(n, "😮");
				return true;
			}
			else if (returned.Success)
			{
				await shell.ReactAsync(n, "🎉");
				if (rnd.Next(100) < 20)
				{
                    await shell.ReplyAsync(n, otsukarePattern.Random());
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
	}
}
