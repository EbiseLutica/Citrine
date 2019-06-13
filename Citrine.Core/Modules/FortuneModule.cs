using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public partial class FortuneModule : ModuleBase
	{
		public async override Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text != null && Regex.IsMatch(n.Text.ToLowerInvariant(), "占|運勢|みくじ|fortune"))
			{
				var r = new Random(n.User.Id.GetHashCode() + DateTime.Now.Day + DateTime.Now.Month - DateTime.Now.Year);

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
				builder.AppendLine($"ラッキーアイテム💎: {GenerateWord()}");

				await shell.ReplyAsync(n, builder.ToString(), $"僕が今日の{core.GetNicknameOf(n.User)}の運勢を占ったよ:");

				return true;
			}

			return false;
		}

		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);

		public static string GenerateWord()
		{
			var sb = new StringBuilder();
			if (rnd.Next(100) > 50)
				sb.Append(ItemPrefixes.Random());
			sb.Append(Items.Random());
			if (rnd.Next(100) > 70)
				sb.Append(ItemSuffixes.Random());
			return sb.ToString();
		}

		static Random rnd = new Random();

		static string GetStar(int value, int maxValue) => new string('★', value) + new string('☆', maxValue - value);

	}

}
