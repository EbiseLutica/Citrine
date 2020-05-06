using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class ReactModule : ModuleBase
	{
		public override int Priority => -1000;

		public static readonly string StatReactCount = "stat.react-count";

		public static readonly string StatBadMouthCount = "stat.bad-mouth-count";

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself?.Id)
				return false;

			if (n.Text == null)
				return false;

			var m = Regex.Match(n.Text, "((?:\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]|[\\:a-z0-9A-Z_]+))を?[投な]げ[てろよ]");
			if (m.Success)
			{
				core.LikeWithLimited(n.User);
				EconomyModule.Pay(n, shell, core);
				await shell.ReactAsync(n, m.Groups[1].Value.Trim());
				core.Storage[n.User].Add(StatReactCount);
				return true;
			}
			else if (IsTerribleText(n.Text))
			{
				core.OnHarassment(n.User);
				await shell.ReactAsync(n, "😥");
				core.Storage[n.User].Add(StatBadMouthCount);
				var rate = core.GetRatingOf(n.User);
				await shell.ReplyAsync(n, (rate == Rating.Hate ? ponkotsuPatternHate : rate == Rating.Normal ? ponkotsuPattern : ponkotsuPatternLove).Random());
				return true;
			}

			return false;
		}

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			await Task.Delay(0);
			if (string.IsNullOrEmpty(n.Text))
				return false;

			// ひどい言葉は見て見ぬ振り
			if (IsTerribleText(n.Text))
				return true;
			return false;
		}

		private bool IsTerribleText(string text)
		{
			return text.IsMatch("ぽんこつ|ポンコツ|バカ|馬鹿|ばか|あほ|アホ|阿呆|間抜け|まぬけ|ごみ|ゴミ|死ね|ブス|ぶす|ぶさいく|ブサイク|不細工|無能|キモ[いイ]|殺す|ハゲ|禿") && !text.IsMatch("(じゃ|では?)な[いく]");
		}

		private static readonly string[] ponkotsuPattern =
		{
			"酷いです...",
			"ひどい...",
			"なんでそういうこと言うんですか.",
			"そういう言葉嫌いです",
			"そういう言葉遣い, 嫌です",
			"そんなこと言われると傷つきます",
			"..."
		};

		private static readonly string[] ponkotsuPatternHate =
		{
			"本当に最低だね",
			"は?",
			"何なの?",
			"いい加減にして.",
			"どこまで私を侮蔑すれば気が済むの?",
			"最低",
			"..."
		};

		private static readonly string[] ponkotsuPatternLove =
		{
			"ひどいよ!",
			"え, 何でそういうこと言うの?",
			"ねえ, 嫌いになったの...?",
			"ひどいよ...",
			"あんまりそういうこと言われると嫌いになっちゃうよ...?",
			"..."
		};
	}
}
