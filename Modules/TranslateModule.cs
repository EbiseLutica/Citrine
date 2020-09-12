using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class TranslateModule : ModuleBase
	{
		public override int Priority => -1000;

		public static readonly string StatTranslatedCount = "stat.translated-count";
		public static readonly string StatRetranslatedCount = "stat.retranslated-count";

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself?.Id)
				return false;

			if (n.Text == null)
				return false;

			foreach (var (pattern, code) in langs)
			{
				var matchNormal = Regex.Match(n.Text.TrimMentions(), $"(.+)を{pattern}[にへ]?翻?訳");
				var matchReposted = Regex.Match(n.Text.TrimMentions(), $"これを?{pattern}[にへ]?翻?訳");
				if (matchReposted.Success && n.Repost != null)
				{
					if (string.IsNullOrEmpty(n.Repost.Text))
					{
						await shell.ReplyAsync(n, "ん, その投稿にはテキストが含まれていないよ? 無いものは翻訳できないよね...");
						return true;
					}
					await TranslateAsync("auto", code, n.Repost.Text, n, shell, core);
					core.Storage[n.User].Add(StatTranslatedCount);
					return true;
				}
				else if (matchNormal.Success)
				{
					await TranslateAsync("auto", code, HttpUtility.UrlEncode(matchNormal.Groups[1].Value), n, shell, core);
					core.Storage[n.User].Add(StatTranslatedCount);
					return true;
				}
			}

			return false;
		}

		public override async Task<bool> OnRepliedContextually(IPost n, IPost? context, Dictionary<string, object> store, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself?.Id)
				return false;

			if (n.Text == null)
				return false;

			foreach (var (pattern, code) in langs)
			{
				var m = Regex.Match(n.Text.TrimMentions(), $"{pattern}[にへ]再翻訳");
				if (m.Success)
				{
					await TranslateAsync((string)store["code"], code, (string)store["result"], n, shell, core);
					core.Storage[n.User].Add(StatRetranslatedCount);
					return true;
				}
			}

			return false;
		}

		private async Task TranslateAsync(string from, string to, string text, IPost n, IShell shell, Server core)
		{
			core.LikeWithLimited(n.User);
			var result = await core.ExecCommand($"/translate {from} {to} {text}");
			var reply = await shell.ReplyAsync(n, result);

			if (reply == null)
				return;

			EconomyModule.Pay(n, shell, core);
			core.LikeWithLimited(n.User);
			core.RegisterContext(reply, this, new System.Collections.Generic.Dictionary<string, object>()
			{
				{ "result", result},
				{ "code", to },
			});
		}

		private readonly (string pattern, string code)[] langs = {
			("アイスランド語", "is"),
			("アイルランド語", "ga"),
			("アゼルバイジャン語", "az"),
			("アフリカーンス語", "af"),
			("アムハラ語", "am"),
			("アラビア語", "ar"),
			("アルバニア語", "sq"),
			("アルメニア語", "hy"),
			("イタリア語", "it"),
			("イディッシュ語", "yi"),
			("イボ語", "ig"),
			("インドネシア語", "id"),
			("ウェールズ語", "cy"),
			("ウクライナ語", "uk"),
			("ウズベク語", "uz"),
			("ウルドゥー?語", "ur"),
			("エストニア語", "et"),
			("エスペラント語", "eo"),
			("オランダ語", "dv"),
			("カザフ語", "kk"),
			("カタルーニャ語", "ca"),
			("ガリシア語", "gl"),
			("ギリシ[アャ]語", "el"),
			("キルギス語", "ky"),
			("グジャラー?ト語", "gu"),
			("(カンボジア|クメール)語", "km"),
			("クルド語", "ku"),
			("クロアチア語", "hr"),
			("コー?サ語", "xh"),
			("コルシカ語", "co"),
			("サモア語", "sm"),
			("ジャワ語", "jv"),
			("(グル|ジョー)ジア語", "ka"),
			("ショナ語", "sn"),
			("シンド語", "sd"),
			("シンハラ語", "si"),
			("スウェーデン語", "sv"),
			("ズールー語", "zu"),
			("スコットランド ?ゲール語", "gd"),
			("スペイン語", "es"),
			("スロバキア語", "sk"),
			("スロベニア語", "sl"),
			("スワヒリ語", "sw"),
			("スンダ語", "su"),
			("セブアノ語", "ceb"),
			("セルビア語", "sr"),
			("ソト語", "st"),
			("ソマリ語", "so"),
			("タイ語", "th"),
			("タガログ語", "tl"),
			("タジク語", "tg"),
			("タミル語", "ta"),
			("チェコ語", "cs"),
			("(チェワ|ニャンジャ)語", "ny"),
			("テルグ語", "te"),
			("デンマーク語", "da"),
			("ドイツ語", "de"),
			("トルコ語", "tr"),
			("ネパール語", "ne"),
			("ノルウェー語", "no"),
			("ハイチ語", "ht"),
			("ハウサ語", "ha"),
			("パシュトー?語", "ps"),
			("バスク語", "eu"),
			("ハワイ語", "haw"),
			("(ハンガリー|マジャル)語", "hu"),
			("パンジャー?ブ語", "pa"),
			("ヒンディー語", "hi"),
			("フィンランド語", "fi"),
			("フランス語", "fr"),
			("フリジア語", "fy"),
			("ブルガリア語", "bg"),
			("ベトナム語", "vi"),
			("ヘブライ語", "he"),
			("ベラルーシ語", "be"),
			("ペルシ[アャ]語", "fa"),
			("ベンガル語", "bn"),
			("ポーランド語", "pl"),
			("ボスニア語", "bs"),
			("ポルトガル語", "pt"),
			("マオリ語", "mi"),
			("マケドニア語", "mk"),
			("マラーティー語", "mr"),
			("マラガシ語", "mg"),
			("マラヤーラム語", "ml"),
			("マルタ語", "mt"),
			("マレー語", "ms"),
			("ミャンマー語", "my"),
			("モンゴル語", "mn"),
			("モン語", "hmn"),
			("ヨルバ語", "yo"),
			("ラオ語", "lo"),
			("ラテン語", "la"),
			("ラトビア語", "lv"),
			("リトアニア語", "lt"),
			("ルーマニア語", "ro"),
			("ルクセンブルク語", "lb"),
			("ロシア語", "ru"),
			("英語", "en"),
			("(韓国|朝鮮)語", "ko"),
			("中国語", "zh"),
			("(中国語)（簡体）", "zh-CN"),
			("(中国語)（繁体）", "zh-TW"),
			("日本語", "ja"),
		};
	}
}
