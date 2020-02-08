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
			if (n.Text != null && n.Text.Contains("じゃんけん"))
			{
				core.LikeWithLimited(n.User);
				var note = await shell.ReplyAsync(n, "いいね〜, じゃあやろう. 最初は✊, じゃんけん――");
				if (note == null)
					return true;
				EconomyModule.Pay(n, shell, core);
				core.RegisterContext(note, this);
				return true;
			}

			return false;
		}

		public override async Task<bool> OnRepliedContextually(IPost n, IPost? context, Dictionary<string, object> store, IShell shell, Server core)
		{
			if (n.Text == null) return false;
			var player = NormalizeHand(n.Text);
			var me = new[] { "✊", "✌", "✋" }[rnd.Next(3)];

			Result result = DoBSPGame(player, me);
			var output = result switch
			{
				Result.Draw => "あいこだ... はーい, あいこで",
				Result.Win => "私の勝ち! " + winMessage.Random().Replace("$user$", core.GetNicknameOf(n.User)),
				Result.Lose => $"私の負け..." + loseMessage.Random().Replace("$user$", core.GetNicknameOf(n.User)),
				_ => $"(Bug) Invalid State {result}",
			};

			output = $"ポン! {me}\n{output}";

			var replied = await shell.ReplyAsync(n, output);
			if (result == Result.Draw && replied != null)
			{
				core.RegisterContext(replied, this);
			}
			return true;
		}

		private static string NormalizeHand(string text)
		{
			return text switch
			{
				"ちょき" => "✌",
				"チョキ" => "✌",
				"✌" => "✌",
				"グー" => "✊",
				"ぐー" => "✊",
				"✊" => "✊",
				"パー" => "✋",
				"ぱー" => "✋",
				"✋" => "✋",
				_ => throw new Exception(),
			};
		}

		private Result DoBSPGame(string player, string citrine)
		{
			return citrine == player ? Result.Draw
					: IsCitrinesWin(player, citrine) ? Result.Win
					: Result.Lose;
		}

		private bool IsCitrinesWin(string p, string c) => (c == "✋" && p == "✊") || (c == "✌" && p == "✋") || (c == "✊" && p == "✌");

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
