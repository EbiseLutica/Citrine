using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class JankenModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.IsReply && n.Text != default && cache.ContainsKey(n.Reply.Id) && cache[n.Reply.Id] == n.User.Id)
			{
				string player;
				// じゃんけん入力
				switch (n.Text.TrimMentions())
				{
					case "ちょき":
					case "チョキ":
					case "✌️":
						player = "✌";
						break;
					case "グー":
					case "ぐー":
					case "✊":
						player = "✊";
						break;
					case "パー":
					case "ぱー":
					case "✋":
						player = "✋";
						break;
					default:
						await shell.ReplyAsync(n, "なにその手. グー/チョキ/パーで選んでほしいな.");
						return true;
				}
				var me = new[] { "✊", "✌", "✋" } [rnd.Next(3)];

				string output;
				Result result = DoJanken(player, me);
				switch (result)
				{
					case Result.Draw:
						output = "あいこだ... はーい, あいこで";
						break;
					case Result.Win:
						output = "僕の勝ち! " + winMessage.Random().Replace("$user$", core.GetNicknameOf(n.User));
						break;
					case Result.Lose:
						output = $"僕の負け..." + loseMessage.Random().Replace("$user$", core.GetNicknameOf(n.User));
						break;
					default:
						output = $"(Bug) Invalid State {result}";
						break;
				}

				output = $"ポン! {me}\n{output}";

				var replied = await shell.ReplyAsync(n, output);
				if (result == Result.Draw)
				{
					cache[replied.Id] = n.User.Id;
				}

				cache.Remove(n.Reply.Id);
				return true;
			}

			if (n.Text != null && n.Text.Contains("じゃんけん"))
			{
				var note = await shell.ReplyAsync(n, "いいね〜, じゃあやろう. 最初は✊, じゃんけん――");
				cache[note.Id] = n.User.Id;
				return true;
			}

			return false;
		}

		private Result DoJanken(string player, string citrine)
		{
			return citrine == player ? Result.Draw
					: IsCitrinesWin(player, citrine) ? Result.Win
					: Result.Lose;
		}

		private bool IsCitrinesWin(string p, string c) => (c == "✋" && p == "✊") || (c == "✌" && p == "✋") || (c == "✊" && p == "✌");

		// シトリンのpostId と 対応する userid のキャッシュ。 IPost.Reply.Id と IPost.User.Id で照合する
		private readonly Dictionary<string, string> cache = new Dictionary<string, string>();
		private readonly Random rnd = new Random();

		private readonly string[] loseMessage = {
			"$user$って強いね. またやろうね.",
			"楽しかった. ありがとう!",
			"悔しい... 次は負けないよ〜.",
			"ぐぐぐ... 次こそは",
			"うぐぐ, $user$, 泣きの一回...😢"
		};

		private readonly string[] winMessage = {
			"たかがじゃんけん, そう思っていませんか!",
			"楽しかった. ありがとう!",
			"$user$, 落ち込まないで...またやろ?",
			"わーい, 勝った."
		};

		private enum Result
		{
			Win,
			Draw,
			Lose,
		}
	}
}
