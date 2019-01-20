using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Citrine.Core.Core
{
	public partial class FortuneModule : ModuleBase
	{
		public async override Task<bool> ActivateAsync(Note n, MisskeyClient mi, Server core)
		{
			if (n.Text != null && Regex.IsMatch(n.Text.ToLowerInvariant(), "占|運勢|みくじ|fortune"))
			{
				var r = new Random(n.UserId.GetHashCode() + DateTime.Now.Day + DateTime.Now.Month - DateTime.Now.Year);

				int love = r.Next(1, 6),
					money = r.Next(1, 6),
					work = r.Next(1, 6),
					study = r.Next(1, 6);

				var result = Math.Min((love + money + work + study) / 2 - 1, results.Length - 1);

				var builder = new StringBuilder();

				builder.AppendLine($"***{results[result]}***");
				builder.AppendLine($"恋愛運❤: {GetStar(love, 5)}");
				builder.AppendLine($"金運💰: {GetStar(money, 5)}");
				builder.AppendLine($"仕事💻: {GetStar(work, 5)}");
				builder.AppendLine($"勉強📒: {GetStar(study, 5)}");
				builder.AppendLine($"ラッキーアイテム💎: {itemPrefixes.Random(r)}{items.Random(r)}");

				await mi.Notes.CreateAsync(
					builder.ToString(),
					n.Visibility,
					cw: $"僕が今日の{(n.User.Name ?? n.User.Username)}さんの運勢を占ったよ: ",
					replyId: n.Id);

				return true;
			}

			return false;
		}

		static string GetStar(int value, int maxValue) => new string('★', value) + new string('☆', maxValue - value);
	}

	public class AdminModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(Note n, MisskeyClient mi, Server core)
		{
			if (n.Text == null)
				return false;

			if (n.Text.Contains("再起動"))
			{
				if (core.IsAdmin(n.User))
				{
					await mi.Notes.CreateAsync(
						"またねー。",
						n.Visibility,
						replyId: n.Id
					);
					// good bye
					Environment.Exit(0);
				}
				else
				{
					var mes = core.GetRatingOf(n.User) == Rating.Partner ? "いくらあなたでも, その頼みだけは聞けない. ごめんね..." : "申し訳ないけど, 他の人に言われてもするなって言われてるから...";
					await mi.Notes.CreateAsync(mes, n.Visibility, replyId: n.Id);
				}
				return true;
			}

			return false;
		}
	}

}