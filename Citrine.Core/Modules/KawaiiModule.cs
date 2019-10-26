using System;
using System.Collections.Generic;
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
				// 自前の可愛いリストを引っ張ってくる
				var kawaiiList = core.GetMyStorage().Get("kawaiiList", new List<string>());
				var input = n.Text.Replace($"@{shell.Myself.Name}", "").Trim();

				const string adjective = "かわいい|可愛い|カワイイ";
				var teacherPattern = Regex.Match(input, $"(.+)は({adjective})[でっ]?す?の?よ?");
				var questionPattern = Regex.Match(input, $"(.+)は({adjective})(と((思|おも)う)|ですか|の|かな)?[?？]");
				var queryPattern = Regex.Match(input, $"({adjective})(もの|物|の|人|子|娘|ひと)(は|って|とは)(何|なに|誰|どなた|何方)?[？?]*");

				if (questionPattern.Success)
				{
					var s = teacherPattern.Groups[1].Value.Trim();
					// 可愛いかどうかを調べる
					var response = kawaiiList.Contains(s) ? "かわいい" : "わかりません";
					await shell.ReplyAsync(n, response);
				}
				else if (teacherPattern.Success)
				{
					var s = teacherPattern.Groups[1].Value.Trim();
					if (kawaiiList.Contains(s))
					{
						// 既に知っているので共感
						await shell.ReplyAsync(n, $"わかる, {s} はかわいい!");
						return true;
					}
					if (s.Contains("シトリン") || s.Contains("しとりん") || s.ToLowerInvariant().Contains("citrine"))
					{
						// 自分を褒めてくれる場合は後続のモジュールが処理してくれることを期待
						return false;
					}

					// 記憶
					kawaiiList.Add(s);
					core.GetMyStorage().Set("kawaiiList", kawaiiList);
                    await shell.ReplyAsync(n, $"{s}はかわいい... 覚えました");
				}
				else if (queryPattern.Success)
				{
					// ランダムに可愛いものを引く
					await shell.ReplyAsync(n, kawaiiList.Random() + " はかわいいです");
				}
				else
				{
					return false;
				}

				return true;
			}

			return false;
		}
	}
}
