using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class EconomyModule : ModuleBase
	{
		public override int Priority => -9000;

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string text)
			{
				if (text.Contains("所持金"))
				{
					var balance = core.Storage[n.User].Get("economy.balance", 0);
					await shell.ReplyAsync(n, $"{core.GetNicknameOf(n.User)}の所持金は, {balance} クォーツです!");
					return true;
				}
				if (text.IsMatch("[買か]い(物|もの)|ク[オォ]ーツショップ"))
				{
					var builder = new StringBuilder();
					builder.AppendLine("ようこそクォーツショップへ!お取り扱いしている商品はこちらです.");
					builder.AppendLine(string.Join("\n\n", ShopItems.Select(i => $"{i.DisplayName} {i.Price}クォーツ\n　{i.Description}")));
					builder.Append("欲しい商品があったら、「〜〜をください」って話しかけてください.");
					await shell.ReplyAsync(n, builder.ToString());
					return true;
				}
				foreach (var item in ShopItems)
				{
					if (text.TrimMentions().IsMatch($"^{Regex.Escape(item.DisplayName)}を(ください|ちょうだい|くれ|頂戴)"))
					{
						string res;
						if (HasItem(n.User, item.Id, core))
						{
							res = item.DisplayName + " は既に持ってるみたいですよ.";
						}
						else if (TryUseMoney(n.User, item.Price, core))
						{
							GiveItem(n.User, item.Id, core);
							res = $"お買い上げありがとうございます! ({item.DisplayName} を手に入れました)";
						}
						else
						{
							res = "お金が足りません.";
						}
						await shell.ReplyAsync(n, res);
						return true;
					}
				}
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

		public static bool TryUseMoney(IUser user, int amount, Server core)
		{
			var storage = core.Storage[user];
			var balance = storage.Get("economy.balance", 0);
			if (balance < amount)
				return false;
			storage.Set("economy.balance", balance - amount);
			return true;
		}

		public static bool HasItem(IUser user, string itemId, Server core) => core.Storage[user].Get("economy.items." + itemId, false);

		public static void GiveItem(IUser user, string itemId, Server core) => core.Storage[user].Set("economy.items." + itemId, true);

		public static void TakeItem(IUser user, string itemId, Server core) => core.Storage[user].Set("economy.items." + itemId, false);

		private static readonly Random rnd = new Random();

		private readonly ShopItem[] ShopItems =
		{
			new ShopItem("fortuneplus", "占い+", "占い機能を増強します。", 1000),
		};
	}

	public struct ShopItem
	{
		public string DisplayName { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public int Price { get; set; }

		public ShopItem(string id, string displayName, string description, int price) => (Id, DisplayName, Description, Price) = (id, displayName, description, price);
	}
}
