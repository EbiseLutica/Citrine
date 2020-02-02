using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public partial class KawaiiModule : ModuleBase
	{
		public override int Priority => -9999;

		public async override Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text != null)
			{
				var storage = core.GetMyStorage();
				var map = storage.Get("kawaiiMap", new Dictionary<string, string>());

				void SaveMap() => storage.Set("kawaiiMap", map);

				if (storage.Has("kawaiiList"))
				{
					// レガシーな可愛いリストがあったらマイグレをする
					var list = storage.Get("kawaiiList", new List<string>());
					list.ForEach(k => map[k] = "かわいい");
					logger.Info($"Migrated {list.Count} kawaii things from the list.");
					storage.Clear("kawaiiList");
				}

				var input = n.Text.Replace($"@{shell.Myself.Name}", "").TrimMentions();

				var adjectiveCombined = $"{adjectiveKawaii}|{adjectiveKawaikunai}|{adjectiveOishii}|{adjectiveMazui}";

				var teacherPattern = Regex.Match(input, $"(.+)(?:は|って)({adjectiveCombined})[でっ]?す?の?よ?");
				var questionPattern = Regex.Match(input, $"(.+)(?:は|って)({adjectiveCombined})(?:と(?:(?:思|おも)う)|ですか|の|かな)?[?？]");
				var queryPattern = Regex.Match(input, $"({adjectiveCombined})(?:もの|物|の|人|子|娘|ひと)(?:(?:は|って|とは|)(?:何|なに|誰|どなた|何方)|を(?:教|おし)えて)?[？?]*");
				var isKawaii = input.IsMatch(adjectiveKawaii);

				if (questionPattern.Success)
				{
					await Task.Delay(4000);
					var target = questionPattern.Groups[1].Value.Trim().ToLowerInvariant();

					var response = map.ContainsKey(target) ? map[target] : "わからない";
					await shell.ReplyAsync(n, response);
				}
				else if (teacherPattern.Success)
				{
					await Task.Delay(4000);
					var targetUnnormalized = teacherPattern.Groups[1].Value.Trim();
					var target = targetUnnormalized.ToLowerInvariant();
					var adj = teacherPattern.Groups[2].Value.Trim();

					if (target.IsMatch("シトリン|しとりん|citrine") && target.IsMatch(adjectiveKawaii))
					{
						// 自分を褒めてくれる場合は後続のモジュールが処理してくれることを期待
						return false;
					}

					// 記憶
					map[target] = adj;
					SaveMap();
					await shell.ReplyAsync(n, $"{targetUnnormalized}は{adj}... 覚えました");
				}
				else if (queryPattern.Success)
				{
					await Task.Delay(4000);
					// ランダムに可愛いものを引く
					var adj = queryPattern.Groups[1].Value.Trim();
					var regex =
						adj.IsMatch(adjectiveKawaii) ? adjectiveKawaii :
						adj.IsMatch(adjectiveKawaikunai) ? adjectiveKawaikunai :
						adj.IsMatch(adjectiveOishii) ? adjectiveOishii :
						adj.IsMatch(adjectiveMazui) ? adjectiveMazui : null;

					var suggest = regex != null ? map.Where(kv => kv.Value.IsMatch(regex)).Select(kv => kv.Key).Random() : null;
					await shell.ReplyAsync(n, suggest != null ? $"{suggest}はどう? {adj}よ" : $"{adj}ものをまだ知らない");
				}
				else
				{
					return false;
				}
				EconomyModule.Pay(n, shell, core);
				core.LikeWithLimited(n.User);
				return true;
			}

			return false;
		}

		private const string adjectiveKawaii = "(?:か[あわ]い|可愛|カワイ)[いイー〜]?";
		private const string adjectiveKawaikunai = "(?:可愛|かわい|カワイ)くない";
		private const string adjectiveOishii = "(?:美味|おい|うま|旨)し?[いー〜]?";
		private const string adjectiveMazui = "(?:美味|おい)しくない|(?:まず|不味)[いイ]";

		private readonly Logger logger = new Logger(nameof(KawaiiModule));
	}
}
