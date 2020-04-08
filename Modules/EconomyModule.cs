using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

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
				var mention = StringExtension.RegexMentions.ToString();
				var m1 = Regex.Match(text, $@"({mention})[にへ](\d+)ク[オォ]ーツを?送金");
				var m2 = Regex.Match(text, $@"(\d+)ク[オォ]ーツを? *({mention}) *[にへ]送金");
				if (m1.Success)
					await TransferQuartzAsync(m1.Groups[1].Value, m1.Groups[2].Value, n, shell, core);
				else if (m2.Success)
					await TransferQuartzAsync(m2.Groups[2].Value, m2.Groups[1].Value, n, shell, core);
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
			int addition = rnd.Next(1, 10) *
				core.GetRatingOf(n.User) switch
				{
					Rating.Partner => 10,
					Rating.BestFriend => 5,
					Rating.Like => 2,
					Rating.Hate => 0,
					_ => 1,
				};
			storage.Add("economy.balance", addition);
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

		private async Task TransferQuartzAsync(string targetMention, string value2, IPost n, IShell shell, Server core)
		{
			var target = await shell.GetUserByNameAsync(targetMention);
			if (target == null)
			{
				await shell.ReplyAsync(n, "ユーザーが見つからなかったため, 送金に失敗しました.");
				return;
			}
			var amount = int.Parse(value2);
			if (!TryUseMoney(n.User, amount, core))
			{
				await shell.ReplyAsync(n, "所持金が送金分よりも少ないため, 送金に失敗しました. あなたの残高は, " + core.Storage[n.User].Get("economy.balance", 0) + "クォーツです.");
				return;
			}
			core.Storage[target].Add("economy.balance", amount);
			await shell.ReplyAsync(n, "送金しました. あなたの残高は, " + core.Storage[n.User].Get("economy.balance", 0) + "クォーツです.");
		}

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
