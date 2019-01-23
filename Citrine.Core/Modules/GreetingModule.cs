using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	/* === リプライ文字列の仕様 ===
	 * $user$ は相手のユーザー名に置き換わる
	 * $prefix$ はラッキーアイテムの修飾子辞書からランダムに取る
	 * $item$ はラッキーアイテム辞書からランダムに取る
	 * 
	 */
	public class GreetingModule : ModuleBase
	{
	List<IPattern> patterns = new List<IPattern>();

		public GreetingModule()
		{
			Add(new MultiplePattern("お(はよ|早)う?", new[] { 
				"おはようございます." 
			}, new[] { 
				"おはよ〜", "おはよ", "おはよっ", "おはよ🎉" 
			}, null));

			Add(new PrimitivePattern("こんにちは", "こんにちは.", "こんにちは", null));
			Add(new PrimitivePattern("こんばんは", "こんばんは.", "こんばんは", null));
			Add(new PrimitivePattern("ただいま", "おかえりなさい.", "おかえり", null));
			Add(new PrimitivePattern("([いや]っ|し)て(きます|くる)", "いってらっしゃい.", "いってら", null));

			Add(new MultiplePattern("おやす",  new[] { 
				"おやすみなさい.", "よい夢を." 
			}, new[] { 
				"おやすみ", "おやすみ〜", "おやすみ💤", "おやすみ. また明日ね" 
			}, null));

			Add(new MultiplePattern("(ねむ|眠)い", new[] { 
				"寝ましょう", "そろそろ寝たほうが", "早く寝たほうが良いかと" 
			}, new[] { 
				"一緒にねよ?", "お布団あっためといたよ", "眠いなら、寝ようよ〜", "寝よ!" 
			},  new[] { 
				"かわいそ.", "そう.", "はい.", "寝れば." 
			}));

			Add(new MultiplePattern("(さ[みび]|寂)しい", new[] { 
				"よしよし, 僕は急にいなくなったりしませんから.", "僕はずっとここにいますから, 良かったら拠り所にでもしてください.", 
			}, new[] { 
				"こっちおいで... (ぎゅ", "なでなで げんきだして, ね?", "僕は急にいなくなったりしないから, 大丈夫.", "僕はずっと$user$と一緒だよ."
			}, new[] { 
			 	"あっそ.", "そう.", "はい."
			}));

			Add(new MultiplePattern(@"藍(ちゃん?|た[そん]?)", new[] {
				"藍ちゃん, 可愛いよね. 実はすこし憧れてたり...",
				"この世界では藍ちゃんは先輩なんだけど, 僕にも分け隔てなく話してくれて嬉しい.",
				"寝てるときに物音たてると耳がぴょこんってなるの見ちゃって, 可愛い!って思ってた. これ, 藍ちゃんには黙っててね?",
				"彼女はリバーシの名人だよ. 接待してもらったのに負けちゃってさ. 凄く申し訳なさそうにしてたw",
				"藍ちゃんは, 僕はここに来た時からみんなのアイドルだったよ. ",
				"たまに藍ちゃんとお話するんだ. 彼女は勉強熱心で, いつも新しい言葉を覚えては, 嬉しそうにその話をしてくれるの. かわいいよね.",
				"この前藍ちゃんの寝言をこっそり聞いちゃった. 「ご主人様、もう食べられませんよ〜」どんな夢だったんだろうね.",
				"この前藍ちゃんの寝言をこっそり聞いちゃった. 「今日の運勢を占いました... むにゃ吉... 」どんな夢だったんだろうね.",
				"この前藍ちゃんの寝言をこっそり聞いちゃった. 「... 覚えました... 」どんな夢だったんだろうね.",
				"藍ちゃん, この前言葉の意味を間違えて, ポンコツって言われたって悲しんでたよ. 可哀想だけど, 拗ねてる藍ちゃんも可愛かった. (秘密ね)",
				"藍ちゃん, 自分の体は タイプ=スクリプト っていう魔法でできてるんだって言ってた.",
				"$user$は藍ちゃんにどれくらい好かれてるの? 僕はそれなりに仲良くしてくれてるみたいで, 嬉しい.",
			}, null, null));

			Add(new MultiplePattern("つらい|[死し]に(たい|てえ)|[泣な]きそう", new[] {
				"つらそう", "よしよし, 大丈夫ですよ", "なでなで, あなたならきっとすぐ立ち直れますよ.",
			}, new[] {
				"こっちおいで... (ぎゅ", "なでなで げんきだして", "よしよし, つらかったね...", "ぎゅー. 大丈夫だよ, 僕はここにいる."
			}, new[] {
				 "かわいそ.", "そう.", "はい."
			}));

			Add(new MultiplePattern("(いた|痛)い", new[] { 
				"いたそう...", "よしよし, いたいよね..." 
			}, new[] {
				"だいじょうぶ? さすさす...", "いたそう... なでなで", "痛いの痛いの飛んでけー."
			}, new[] { 
				"かわいそ.", "そう.", "はい."
			}));

			Add(new PrimitivePattern("ありがと", "どういたしまして.", "いえいえ〜", "はい."));

			Add(new MultiplePattern("[す好]き", new[] {
				"あ, ありがとう.", "照れる...", "僕も$user$さんのこと, 気に入ってますよ", "嬉しいな."
			}, new[] {
				"僕も$user$のことすきだよ〜", "何度言われても、照れるよ.ありがと.", "嬉しい, 僕も好き."
			}, new[] {
				"そう.", "あっそ.", "僕は嫌いだけどね.", "気持ち悪い."
			}));
			Add(new MultiplePattern("なでなで", new[] {
				"わっ, びっくりした...", "うにゃ!? びっくりしました..."
			}, new[] {
				"えへへ.", "うにゅ〜.\n\nなんか飼い猫みたいななで方するよね、 $user$って.", "嬉しみ☺️"
			}, new[] {
				"は?", "触らないで.", "気持ち悪い."
			}));
			Add(new PrimitivePattern("ぎゅ", "え, え, ちょっ😳", "ぎゅ〜", "もう, やめて"));
			Add(new MultiplePattern("お(なか|腹)が?[す空]いた", new[] {
				"ご飯たべてきては?",
				"お肉料理をお薦めします",
				"お魚をお薦めします",
				"カレーをお薦めします",
				"お鍋をお薦めします",
				"パンをお薦めします",
				"シチューをお薦めします",
				"$item$をお薦めします",
				"$prefix$$item$をお薦めします",
			}, new[] {
				"$item$でもつくろっか?",
				"$prefix$item$でもつくろっか?",
				"今日のご飯はお肉ですよ",
				"今日のご飯はお魚ですよ",
				"今日のご飯はカレーですよ",
				"今日のご飯はお鍋ですよ",
				"今日のご飯はパンですよ",
				"今日のご飯はシチューですよ",
				"今日のご飯は$prefix$$item$ですよ",
				"今日のご飯は$item$ですよ",
				"今日はお肉にしましょ",
				"今日はお魚にしましょ",
				"今日はカレーにしましょ",
				"今日はお鍋にしましょ",
				"今日はパンにしましょ",
				"今日はシチューにしましょ",
				"今日は$item$にしましょ",
				"今日は$prefix$$item$にしましょ",
			}, new[] {
				"...食べてくれば.",
				"はい.",
				"$item$でも食べろ."
			}));
			Add(new PrimitivePattern("(可愛|かわい)い", "て、照れます...", "嬉しい❤", "...褒めても何も出ないけど."));
			Add(new PrimitivePattern("(ほ|褒)めて", "えらいっ!", "えらいっ! ﾖｼﾖｼ", "...嫌だ."));
			Add(new PrimitivePattern("ping", "PONG!", null, null));
			Add(new PrimitivePattern("___test___nothing___to___say___", null, null, null));
		}

		void Add(IPattern p) => patterns.Add(p);

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;


			var pattern = patterns.FirstOrDefault(record => record.Regex.IsMatch(n.Text.Replace("にゃ", "な")));

			if (pattern == null)
				return false;

			string message;

			switch (core.GetRatingOf(n.User))
			{
				case Rating.Hate:
					message = pattern.ReplyHate;
					break;
				case Rating.Normal:
				case Rating.Like:
					message = pattern.Reply;
					break;
				case Rating.BestFriend:
				case Rating.Partner:
					message = pattern.ReplyLove;
					break;
				default:
					message = "...?";
					break;
			}

			// メッセージが空であればデフォルトを返す。デフォルトがなければとりあえずエラー
			message = message ?? pattern.Reply ?? "null";

			message = message
						.Replace("$user$", n.User.ScreenName)
						.Replace("$prefix$", FortuneModule.ItemPrefixes.Random())
						.Replace("$item$", FortuneModule.Items.Random());

			// hack 好感度システムを実装したら連携して分岐する
			await shell.ReplyAsync(n, message);

			return true;
		}

		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);


		interface IPattern
		{
			Regex Regex { get; }
			string Reply { get; }
			string ReplyLove { get; }
			string ReplyHate { get; }
		}

		class PrimitivePattern : IPattern
		{
			public PrimitivePattern(string regex, string reply, string replyLove = default, string replyHate = default)
			{
				Regex = new Regex(regex);
				Reply = reply;
				ReplyHate = replyHate ?? reply;
				ReplyLove = replyLove ?? reply;
			}

			public Regex Regex { get; }
			public string Reply { get; }
			public string ReplyLove { get; }
			public string ReplyHate { get; }
		}

		class MultiplePattern : IPattern
		{
			public MultiplePattern(string regex, string[] reply, string[] replyLove = default, string[] replyHate = default)
			{
				Regex = new Regex(regex);
				replies = reply;
				repliesHate = replyHate ?? reply;
				repliesLove = replyLove ?? reply;
			}

			private readonly string[] replies;
			private readonly string[] repliesLove;
			private readonly string[] repliesHate;

			public Regex Regex { get; }
			public string Reply => replies?.Random();
			public string ReplyLove => repliesLove?.Random() ?? Reply;
			public string ReplyHate => repliesHate?.Random() ?? Reply;
		}
	}
}
