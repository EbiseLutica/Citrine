using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disboard.Misskey;
using Disboard.Misskey.Models;

namespace Citrine
{
	public class GreetingModule : ModuleBase
	{
		(string pattern, string reply, string replyLove, string replyHate)[] greetingTable =
		{
			("お(はよ|早)う?", "おはようございます.", "おはよ", null),
			("こんにちは", "こんにちは.", "こんにちは〜", null),
			("こんばんは",  "こんばんは.", "こんばんは〜", null),
			("ただいま",  "おかえりなさい.", "おかえり〜！", null),
			("([いや]っ|し)て(きます|くる)",  "いってらっしゃい.", "いってら〜！", null),
			("おやす", "おやすみなさい.", "おやす〜", null),
			("(ねむ|眠)い", "寝ましょ?", "一緒にねよ?", "...寝れば?"),
			("つらい", "なでなで", "だ, だいじょうぶ? こっちおいで...", "かわいそ."),
			("(いた|痛)い", "大丈夫ですか?", "だいじょうぶ? さすさす...", "かわいそ."),
			("ありがと", "どういたしまして.", "いえいえ〜", "はい."),
			("[す好]き", "あ, ありがとう.", "僕もすきだよ〜", "そう."),
			("なでなで", "わっ, びっくりした...", "えへへ.", "は?"),
			("ぎゅ", "え, え, ちょっ", "ぎゅ〜.", "もう, やめて"),
			("お(なか|腹)[す空]いた", "ご飯たべてきな〜?", "なんかつくろっか?", "...食べてくれば."),
			("(可愛|かわい)い", "て、照れます...", "嬉しい❤", "...褒めても何も出ないけど."),
			("(ほ|褒)めて", ":erait:", "***:erait:***", "...嫌だ."),
			("ping", "PONG!", null, null),
		};

		public override async Task<bool> ActivateAsync(Note n, MisskeyClient mi, Citrine core)
		{
			if (n.Text == null)
				return false;

			var (pattern, reply, replyLove, replyHate) = greetingTable.FirstOrDefault(record => Regex.IsMatch(n.Text.Replace("にゃ", "な"), record.pattern));

			if (pattern == null)
				return false;

			await Task.Delay(1000);

			string message;

			switch (core.GetRatingOf(n.User))
			{
				case Rating.Hate:
					message = replyHate;
					break;
				case Rating.Normal:
				case Rating.Like:
					message = reply;
					break;
				case Rating.BestFriend:
				case Rating.Partner:
					message = replyLove;
					break;
				default:
					message = "...?";
					break;
			}

			// メッセージが空であればデフォルトを返す。デフォルトがなければとりあえずエラー
			message = message ?? reply ?? "バグ";

			// hack 好感度システムを実装したら連携して分岐する
			await mi.Notes.CreateAsync(
				message,
				n.Visibility,
				replyId: n.Id
			);

			return true;
		}
	}
}
