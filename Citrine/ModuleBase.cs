using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Disboard.Misskey;
using Disboard.Misskey.Models;

namespace Citrine
{
	/// <summary>
	/// Citrine のモジュールベース。
	/// </summary>
	public abstract class ModuleBase
	{
		public virtual int Priority => 0;

		public virtual async Task<bool> ActivateAsync(Note n, MisskeyClient mi, Citrine core) => false;

		public virtual async Task<bool> OnTimelineAsync(Note n, MisskeyClient mi, Citrine core) => false;
	}

	public class GreetingModule : ModuleBase
	{
		(string pattern, string reply, string replyLove, string replyHate)[] greetingTable =
		{
			("お(はよ|早)う?", "おはようございます.", "おはよ", null),
			("こんにちは", "こんにちは.", "こんにちは〜", null),
			("こんばんは",  "こんばんは.", "こんばんは〜", null),
			("おやす", "おやすみなさい.", "おやす〜", null),
			("(ねむ|眠)い", "寝ましょう?", "一緒にねよ?", "...寝れば?"),
			("つらい", "なでなで", "だいじょうぶ? こ, こっちおいで...", "かわいそ."),
			("(いた|痛)い", "大丈夫ですか?", "だいじょうぶ? さすさす...", "かわいそ."),
			("ありがと", "どういたしまして.", "いえいえ〜", "はい."),
			("[す好]き", "あ, ありがとう.", "僕もすきだよ〜", "そう."),
			("なでなで", "わっ, びっくりした...", "えへへ.", "は?"),
			("ぎゅ", "え, え, ちょっ", "ぎゅ〜.", "もう, やめて"),
			("お(なか|腹)[す空]いた", "ご飯たべましょ〜.", "なんかつくりましょうか!", "...食べてくれば."),
			("(可愛|かわい)い", "て、照れます...", "嬉しい❤", "...褒めても何も出ないけど."),
			("ping", "PONG!", null, null),
		};

		public override async Task<bool> ActivateAsync(Note n, MisskeyClient mi, Citrine core)
		{
			if (n.Text == null)
				return false;

			var rec = greetingTable.FirstOrDefault(record => Regex.IsMatch(n.Text.Replace("にゃ", "な"), record.pattern));

			if (rec.pattern == null)
				return false;

			await Task.Delay(1000);

			// hack 好感度システムを実装したら連携して分岐する
			await mi.Notes.CreateAsync(
				core.IsMaster(n.User) ? rec.replyLove ?? rec.reply : rec.reply,
				replyId: n.Id
			);

			return true;
		}
	}

	public class VoteModule : ModuleBase
	{
		Random rnd = new Random();
		public override async Task<bool> OnTimelineAsync(Note n, MisskeyClient mi, Citrine core)
		{
			if (n.Poll == null)
				return false;

			await Task.Delay(1000);

			// ランダムで投票する
			await mi.Notes.Polls.VoteAsync(n.Id, rnd.Next(n.Poll.Choices.Count()));

			// 多分競合しないから常にfalse
			return false;
		}
	}
}
