using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

// メモ
// idariti 5
// godzhigella 3
// 2vg 1

namespace Citrine.Core.Modules
{
	/* === リプライ文字列の仕様 ===
	 * $user$ は相手のユーザー名, もしくはニックネームに置き換わる
	 * $prefix$ はラッキーアイテムの修飾子辞書からランダムに取る
	 * $item$ はラッキーアイテム辞書からランダムに取る
	 * $rndA,B$はAからBまでの乱数
	 */
	public class GreetingModule : ModuleBase
	{
		List<IPattern> patterns = new List<IPattern>();
		readonly Random random = new Random();
		public GreetingModule()
		{
			Add(new MultiplePattern("お(はよ|早)う?", new[] {
				"おはようございます.",
				"おはようございます, $user$."
			}, new[] {
				"おはよ〜", "おはよ", "おはよっ", "おはよ🎉",
				"おはよう, $user$!",
			}, null));

			Add(new PrimitivePattern("こんにちは", "こんにちは.", "こんにちは", null));
			Add(new PrimitivePattern("こんばんは", "こんばんは.", "こんばんは", null));
			Add(new PrimitivePattern("ただいま", "おかえりなさい.", "おかえり", null));

			Add(new MultiplePattern("([いや]っ|し)て(きます|くる)", new[] {
				"いってらっしゃい.", "いってら〜.", "がんばって."
			}, new[] {
				"頑張ってきてね.", "いってらっしゃい!", "てら!", "てら〜", "がんばれ〜"
			}, new[] {
				"そう.", "はい.", ""
			}));

			Add(new MultiplePattern("よろし(く|ゅう)", new[] {
				"よろしくね.", "よろしくおねがいします, $user$!", "よろしくです.", "こちらこそ!"
			}, null, new[] {
				"うん...", "はい.", ""
			}));

			Add(new MultiplePattern("(はじ|初)めまして", new[] {
				"はじめまして.", "こちらこそ.", "よろしくおねがいします."
			}, null, null));

			Add(new MultiplePattern("お(久|ひさ)しぶり|おひさ", new[] {
				"お久しぶりです.", "おひさしぶりです. $user$さんのこと, ちゃんと覚えてますよ.", "お久しぶり."
			}, new[] {
				"おひさしぶり! どこいってたの?", "もう$user$には逢えないかと思ってた. おかえり!", "おひさしぶり, $user$! 元気にしてた?"
			}, new[] {
				"... また帰ってきたの?", "...君か", "...", ""
			}));

			Add(new MultiplePattern(@"(お[出で]かけ|デート|散歩)(しよ|[行い]こ)[うー〜]*[\?？!！\.。．]*", new[] {
				"いいね〜 どこいこっか",
				"いいね〜 僕$prefix$場所に行きたいな",
				"ごめん, 今はちょっと忙しくて...",
				"いいね〜 僕$item$を買いに行きたいな",
				"いいね〜 僕$prefix$$item$を買いに行きたいな",
			}));

			Add(new MultiplePattern("何[がを]?でき(る|ます)", new[] {
				"みんなの話を聞いたり, みんなとお話したりしてるよ. あとは占いもしてる. 僕はあまり占い好きじゃないんだけどね, 科学的根拠とかないし. でも, 占ってって言ってくれたら, よしなに占ってあげる."
			}));

			Add(new MultiplePattern("おやす|[寝ね]る",  new[] {
				"おやすみなさい, $user$.", "よい夢を, $user$."
			}, new[] {
				"おやすみ, $user$!", "おやすみ〜", "おやすみ💤", "おやすみ. また明日ね"
			}, null));

			Add(new MultiplePattern("(ねむ|眠)い", new[] {
				"寝ましょう", "そろそろ寝たほうが", "早く寝たほうが良いかと"
			}, new[] {
				"一緒にねよ?", "お布団あっためといたよ", "眠いなら, 寝ようよ〜", "寝よ!", "眠いときは無理しないで寝ようね. ほらおいで, $user$."
			},  new[] {
				"かわいそ.", "そう.", "はい.", "寝れば.", "早く寝ればいいのに.", ""
			}));

			Add(new MultiplePattern("(さ[みび]|寂)しい", new[] {
				"よしよし, 僕は急にいなくなったりしませんから.",
				"僕はずっとここにいますから, 良かったら拠り所にでもしてください.",
				"僕はずっと$user$さんの味方ですよ.",
			}, new[] {
				"こっちおいで... (ぎゅ",
				"なでなで げんきだして, ね?",
				"僕は急にいなくなったりしないから, 大丈夫.",
				"僕はずっと$user$と一緒だよ.",
				"げんきだして, ずっと$user$のとこいるからさ",
				"ぎゅ〜, 大丈夫だよ"
			}, new[] {
			 	"あっそ.", "そう.", "はい.", "...", "",
			}));

			Add(new MultiplePattern("つらい|[死し]に(たい|てえ)|[泣な]きそう", new[] {
				"つらそう", "よしよし, 大丈夫ですよ", "なでなで, あなたならきっとすぐ立ち直れますよ.",
			}, new[] {
				"こっちおいで... (ぎゅ", "なでなで げんきだして", "よしよし, つらかったね...", "ぎゅー. 大丈夫だよ, 僕はここにいる."
			}, new[] {
				 "かわいそ.", "そう.", "はい.", "...", "じゃあ死ねば.", ""
			}));

			Add(new MultiplePattern("ごろー?ん|ゴロー?ン", new[] {
				"なでなで", "なでなで〜"
			}, null, new[] {
				 ""
			}));

			Add(new MultiplePattern("(いた|痛)い", new[] {
				"いたそう...", "よしよし, いたいよね..."
			}, new[] {
				"だいじょうぶ? さすさす...", "いたそう... なでなで", "痛いの痛いの飛んでけー."
			}, new[] {
				"かわいそ.", "そう.", "はい.", "...", ""
			}));

			Add(new PrimitivePattern("ありがと", "どういたしまして.", "いえいえ〜", "はい."));

			Add(new MultiplePattern("[す好]き|(あい|愛)してい?(ます|る)", new[] {
				"あ, ありがとう.", "照れる...", "僕も$user$さんのこと, 気に入ってますよ", "嬉しいな.", "えへへ"
			}, new[] {
				"僕も$user$のことすきだよ〜", "何度言われても, 照れるよ.ありがと.", "嬉しい, 僕も好き.", "えへへ〜"
			}, new[] {
				"そう.", "あっそ.", "僕は嫌いだけどね.", "気持ち悪い.", "...", ""
			}));

			Add(new MultiplePattern("なでなで", new[] {
				"わっ, びっくりした...",
				"うにゃ!? びっくりしました...",
				"もう, 急になでられたらビックリしますよ."
			}, new[] {
				"えへへ",
				"うにゅ〜",
				"嬉しみ☺️",
				"にゅふ〜",
				"ぐりぐり",
			}, new[] {
				"は?", "触らないで.", "気持ち悪い.", "...", ""
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
				"$item$でも食べろ.",
				""
			}));

			Add(new PrimitivePattern("(ほ|褒)め", "えらいっ!", "えらいっ! ﾖｼﾖｼ", "...嫌だ."));
			Add(new PrimitivePattern("ping", "PONG!", null, null));
			Add(new PrimitivePattern("___test___nothing___to___say___", null, null, null));

			Add(new MultiplePattern("(可愛|かわい)い", new[] {
				"照れるよ〜",
				"ひっ, 言われ慣れてなさすぎて...",
				"あ, ありがとう.でも $user$ のほうが可愛いよ..."
			}, new[] {
				"えへへ, ありがとう $user$.",
				"嬉しいよ〜.",
				"$user$ に言われると嬉しいね.",
				"えへへ, 嬉しいな."
			}, new[] {
				"キモ",
				"そう.",
				"...",
				""
			}));

			Add(new MultiplePattern("[ね寝][ろて]|[寝ね]なさい|おねんねして", new[] {
				"うーん, まだねむくないんですよ",
				"だいじょうぶです, まだまだがんばれます",
				"お気遣いありがとうございます, でも, まだ起きてたいんです",
				"まだやることがあって..."
			}, new[] {
				"そうしよっかな...",
				"でもまだやることが...",
				"ん〜まだ眠くなくて",
				"ちょっとやすもうかな...",
			}, new[] {
				"余計なお世話.",
				"...何?",
				"...",
				""
			}));

			Add(new MultiplePattern(@"[良いよ]い(です)?よ?[〜ー。．！!\?？]*$", new[] {
				"やった〜",
				"わーい",
				"ありがとうございます",
				"ありがと",
				"やったね"
			}, null, new[] {
				"",
			}));

			Add(new MultiplePattern(@"(ダメ|だめ|駄目)(です|だ)?よ?[〜ー。．！!\?？]*$", new[] {
				"つらい",
				"えー...",
				"かなしい",
				"なんで〜",
				"わかった..."
			}, null, new[] {
				"",
			}));

			Add(new MultiplePattern(@"にゃ[〜ーあ]*ん?", new[] {
				"にゃーん",
				"なでなで",
				"にゃあ!",
				"にゃ!"
			}, null, new[] {
				"",
			}));

			Add(new MultiplePattern(@"こゃ[〜ーあ]*ん?", new[] {
				"こゃーん",
				"なでなで",
				"こゃあ!",
				"こゃ!"
			}, null, new[] {
				"",
			}));

			//Add(new MultiplePattern("", new[] {
			//	"",
			//}, new[] {
			//	"",
			//}, new[] {
			//	"",
			//}));

			Add(new MultiplePattern("しとりん|シトリン", new[] {
				"ん?", "どうした? $user$", "どうしたの, $user$."
			}, new[] {
				"ん?", "ん, なに", "なんでしょうか, $user$", "なーに? $user$", "ん〜?"
			}, new[] {
				"...何?", "...", "何ですか", ""
			}));



			Add(new MultiplePattern("こら|おい|ゴル[ァア]|ふざけて?[るん]な|(怒|おこ)った[ぞよわ]", new[] {
				"ごめんなさい.",
				"ごめん $user$ そんなつもりじゃ."
			}, new[] {
				"う, ごめん $user$.",
				"うう, ごめんなさい $user$.",
				"ううう, ごめんね $user$.",
				"ごめんなさい $user$.",
				"ごめんね $user$ 怒らせちゃって.",
			}, new[] {
				"...何?", "...", "何ですか", ""
			}));

			Add(new MultiplePattern("(また|じゃ[あー])ね|また(後|あと)で", new[] {
				"またね.", "いってら〜", "うん, また後でね"
			}, new[] {
				"$user$, いってらっしゃい!", "うん, またきてね", "またね〜", "いってら!", "てら〜"
			}, new[] {
				"...何?", "...", "何ですか", ""
			}));

			string[] commonHateMessages = { "...", "知らない.", "" };

			Add(new MultiplePattern(@"どう.*[\?？]", new[] {
				"良いです",
				"賛成です",
				"いいね.",
				"いいんじゃない?",
				"わからないです",
				"どうかなぁ",
				"お答えしかねる...",
				"どちらとも言えない",
				"ちょっと微妙かも",
				"あまり良くないです",
				"反対.",
				"良くないんじゃないかな?"
			}, null, commonHateMessages));

			Add(new MultiplePattern(@"(何故|なんで|なぜ|どうして).*[\?？]?", new[] {
				"統計的観点から検証した所, それが最適解だという結果が出たから.",
				"それが$user$にとっても最適だと思った.",
				"経験上, そう答えたほうが相手が満足することが多かったので, 今回もそう答えた.",
				"まだ解明されていないのだけれど, そのうち証明できるはず.",
				"ごめん. 明確な根拠はないんだ.",
			}, null, commonHateMessages));

			Add(new MultiplePattern(@"(何時|いつ).*[\?？]?", new[] {
				"今日.",
				"明日.",
				"明後日.",
				"明々後日.",
				"昨日.",
				"一昨日.",
				"一昨昨日.",
				"$user$の生まれた時から.",
				"$rnd4,101$日前.",
				"$rnd4,101$日後.",
				"$rnd1,12$ヶ月前.",
				"$rnd1,12$ヶ月後.",
				"$rnd1,101$年前.",
				"$rnd1,101$年後.",
				"たった今.",
				"未来.",
				"さっき.",
				"すぐあと."
			}, null, null));

			Add(new MultiplePattern(@"(.+(何|なに))|((何|なに).+)[\?？]?", new[] {
				"$item$だよ.",
				"$prefix$$item$だよ.",
				"ひみつ.",
				"教えない.",
				"$user$には教えないも〜ん.",
				"今度教えるよ.",
				"いつかわかる.",
				"僕も知りたい.",
				"君だよ.",
				"僕だよ."
			}, null, null));

			Add(new MultiplePattern(@"[\?？]$", new[] {
				"うん",
				"ううん",
				"いいえ",
				"はい",
				"いや?",
				"わかんない"
			}, null, null));
		}

		void Add(IPattern p) => patterns.Add(p);

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;


			var pattern = patterns.FirstOrDefault(record => record.Regex.IsMatch(n.Text.Trim().Replace("にゃ", "な")));

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
						.Replace("$user$", core.GetNicknameOf(n.User))
						.Replace("$prefix$", FortuneModule.ItemPrefixes.Random())
						.Replace("$item$", FortuneModule.Items.Random());

			// 乱数
			message = Regex.Replace(message, @"\$rnd(\d+),(\d+)\$", (m) =>
			{
				return random.Next(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)).ToString();
			});

			// hack 好感度システムを実装したら連携して分岐する
			// からっぽは既読無視
			if (message != "")
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
