using System;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class EconomyModule : ModuleBase
	{

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string text)
			{
				if (text.Contains("所持金"))
				{
					var balance = core.Storage[n.User].Get("economy.balance", 0);
					await shell.ReplyAsync(n, $"{core.GetNicknameOf(n.User)}の所持金は, {balance} クォーツです!");
				}
				if (text.IsMatch("[買か]い(物|もの)"))
				{
					await shell.ReplyAsync(n, "クォーツショップはまだ開店準備中だよ. もう少しだけ待っててね");
				}
				return true;
			}
			return false;
		}

		public static void Pay(IPost n, IShell shell, Server core)
		{
			// 好感度に応じて、ランダムな量のお金をあげる
			var storage = core.Storage[n.User];
			var balance = storage.Get("economy.balance", 0);
			int addition = rnd.Next(1, 10) *
				core.GetRatingOf(n.User) switch
				{
					Rating.Partner => 10,
					Rating.BestFriend => 5,
					Rating.Like => 2,
					Rating.Hate => 0,
					_ => 1,
				};
			storage.Set("economy.balance", balance + addition);
		}

		private static readonly Random rnd = new Random();
	}
}
