using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	public class ApexLegendsMapRotationModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;

			var m = pattern.Match(n.Text);
			if (!m.Success) return false;

			var key = Environment.GetEnvironmentVariable("APEX_LEGENDS_API_KEY");
			if (key == null)
			{
				await shell.ReplyAsync(n, "今はちょっとわからないかな…");
				return true;
			}

			var isNext = ToTense(m.Groups[1].Value) == Tense.Next;
			var mode = ToMode(m.Groups[2].Value);

			if (mode == GameMode.Unknown)
			{
				await shell.ReplyAsync(n, $"えーと，Apexに{m.Groups[2].Value}モードってあったっけ…わからないので答えられないです．");
				return true;
			}

			try
			{
				var resString = await (await Server.Http.GetAsync(string.Format(endpoint, key))).Content.ReadAsStringAsync();

				if (resString == null)
				{
					await shell.ReplyAsync(n, "謎のエラー");
				}

				var res = JsonConvert.DeserializeObject<ApiMapRotationResponse>(resString);

				var post = await shell.ReplyAsync(n, GenerateText(res, isNext, mode));

				if (post != null)
				{
					core.RegisterContext(post, this, new Dictionary<string, object>()
					{
						{ "res", res},
						{ "isNext", isNext },
						{ "mode", mode },
					});
				}
			}
			catch (HttpRequestException e)
			{
				await shell.ReplyAsync(n, "サーバーが死んでるかも．");
				logger.Error(e.GetType().Name);
				logger.Error("追加情報: " + e.Message);
				if (e.StackTrace != null) logger.Error(e.StackTrace);
			}


			core.LikeWithLimited(n.User);
			return true;
		}

		public override async Task<bool> OnRepliedContextually(IPost n, IPost? context, Dictionary<string, object> store, IShell shell, Server core)
		{
			if (n.Text == null || !n.Text.IsMatch("次|つぎ")) return false;

			var res = (ApiMapRotationResponse)store["res"];
			var isNext = (bool)store["isNext"];
			var mode = (GameMode)store["mode"];

			await shell.ReplyAsync(n, isNext ? "その次はわからない..." : GenerateText(res, true, mode));
			return true;
		}

		private Tense ToTense(string value)
		{
			return value switch {
				"今" or "いま" or "現在" => Tense.Current,
				"次" or "つぎ" => Tense.Next,
				_ => Tense.All,
			};
		}

		private GameMode ToMode(string value)
		{
			if (string.IsNullOrEmpty(value)) return GameMode.All;
			if (value.Contains("コントロール")) return GameMode.Control;

			if (value.Contains("アリーナ"))
			{
				return value.Contains("ランク") ? GameMode.RankedArena : GameMode.Arena;
			}

			if (value.Contains("カジュアル")) return GameMode.Casual;
			if (value.Contains("ランク")) return GameMode.Ranked;

			return GameMode.Unknown;
		}

		private string ToString(GameMode mode)
		{
			return mode switch {
				GameMode.All => "全て",
				GameMode.Casual => "カジュアル",
				GameMode.Ranked => "ランク",
				GameMode.Arena => "アリーナ",
				GameMode.RankedArena => "アリーナランク",
				GameMode.Control => "コントロール",
				_ => "不明",
			};
		}

		private string Translate(string? mapName)
		{
			return mapName switch {
				"Kings Canyon" => "キングスキャニオン",
				"World's Edge" => "ワールズエッジ",
				"Olympus" => "オリンパス",
				"Storm Point" => "ストームポイント",

				"Party Crasher" => "パーティクラッシャー",
				"Phase Runner" => "フェーズランナー",
				"Overflow" => "オーバーフロー",
				"Encore" => "アンコール",
				"Habitat" => "生息地4",
				"Drop Off" => "ドロップオフ",

				"Hammond Labs" => "ハモンド研究所",
				"Barometer" => "バロメーター",
				"Caustic Treatment" => "コースティックの処理施設",
				null => "無し",
				_ => mapName,
			};
		}

		private string GenerateText(ApiMapRotationResponse res, bool isNext, GameMode mode)
		{
			Record? Get(CurrentNext? cn) => isNext ? cn?.Next : cn?.Current;
			var tense = isNext ? "次" : "今";
			if (mode == GameMode.All)
			{
				return $@"{tense}のマップは，それぞれ
カジュアル: {Translate(Get(res.BattleRoyale)?.MapName)}
ランク: {Translate(Get(res.Ranked)?.MapName)}
アリーナ: {Translate(Get(res.Arenas)?.MapName)}
アリーナランク: {Translate(Get(res.ArenasRanked)?.MapName)}
コントロール: {Translate(Get(res.Control)?.MapName)}

だよ．";
			}
			var rec = mode switch
			{
				GameMode.Casual => Get(res.BattleRoyale),
				GameMode.Ranked => Get(res.Ranked),
				GameMode.Arena => Get(res.Arenas),
				GameMode.RankedArena => Get(res.ArenasRanked),
				GameMode.Control => Get(res.Control),
				_ => null,
			};

			if (rec == null) return $"{tense}の{ToString(mode)}のマップはわからない...";

			var remaining = rec.RemainingInMinutes > 0 ? (rec.RemainingInMinutes + "分") : rec.RemainingInSeconds != null ? (rec.RemainingInSeconds + "秒") : null;

			return $"{tense}の{ToString(mode)}のマップは{Translate(rec.MapName)}だよ．{(remaining == null ? "" : $"あと{remaining}で次のマップに変わるよ．")}";
		}

		private readonly string endpoint = "https://api.mozambiquehe.re/maprotation?version=5&auth={0}";
		private readonly Regex pattern = new Regex("(?:エペ|apex)の(?:(今|いま|次|現在|つぎ)の?)?(?:(.+)の?)?マップ", RegexOptions.IgnoreCase);
        private readonly Logger logger = new Logger(nameof(ApexLegendsMapRotationModule));

		enum Tense
		{
			All,
			Current,
			Next,
		}

		enum GameMode
		{
			All,
			Casual,
			Ranked,
			Arena,
			RankedArena,
			Control,
			Unknown,
		}

		class ApiMapRotationResponse
		{
			[JsonProperty("battle_royale")]
			public CurrentNext? BattleRoyale { get; set; }
			[JsonProperty("arenas")]
			public CurrentNext? Arenas { get; set; }
			[JsonProperty("ranked")]
			public CurrentNext? Ranked { get; set; }
			[JsonProperty("arenasRanked")]
			public CurrentNext? ArenasRanked { get; set; }
			[JsonProperty("control")]
			public CurrentNext? Control { get; set; }
		}

		class CurrentNext
		{
			[JsonProperty("current")]
			public Record? Current { get; set; }
			[JsonProperty("next")]
			public Record? Next { get; set; }
		}

		class Record
		{
			[JsonProperty("start")]
			public int? StartUnixTime { get; set; }
			[JsonProperty("end")]
			public int? EndUnixTime { get; set; }
			[JsonProperty("map")]
			public string? MapName { get; set; }
			[JsonProperty("remainingSecs")]
			public int? RemainingInSeconds { get; set; }
			[JsonProperty("remainingMins")]
			public int? RemainingInMinutes { get; set; }
		}
	}
}
