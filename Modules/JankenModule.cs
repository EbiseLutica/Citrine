using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class JankenModule : ModuleBase
	{
		public static readonly string StatWinCount = "stat.janken.win-count";
		public static readonly string StatLoseCount = "stat.janken.lose-count";
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text != null && n.Text.Contains("じゃんけん"))
			{
				core.LikeWithLimited(n.User);
				var note = await shell.ReplyAsync(n, "負けませんよ．最初は✊，じゃんけん――");
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
			var player = NormalizeHand(n.Text.TrimMentions());
			if (player == null)
			{
				var r = await shell.ReplyAsync(n, "ちゃんと手を出してね．もしちゃんと出してるのにって思ったら，「グー，チョキ，パー」か，肌の色が黄色な手の絵文字であることを確認してね.");
				if (r == null) return true;
				core.RegisterContext(r, this);
				return true;
			}
			var me = new[] { "✊", "✌", "✋" }[rnd.Next(3)];

			Result result = DoBSPGame(player, me);
			var output = result switch
			{
				Result.Draw => "あーいこで",
				Result.Win => "私の勝ちです！" + winMessage.Random().Replace("$user$", core.GetNicknameOf(n.User)),
				Result.Lose => $"私の負けです．" + loseMessage.Random().Replace("$user$", core.GetNicknameOf(n.User)),
				_ => $"(Bug) Invalid State {result}",
			};

			output = $"ポン! {me}\n{output}";

			var storage = core.Storage[n.User];

			if (result == Result.Win)
				storage.Add(StatWinCount);
			else if (result == Result.Lose)
				storage.Add(StatLoseCount);

			var replied = await shell.ReplyAsync(n, output);
			if (result == Result.Draw && replied != null)
			{
				core.RegisterContext(replied, this);
			}
			return true;
		}

		private static string? NormalizeHand(string text)
		{
			return text switch
			{
				"ちょき" => "✌",
				"チョキ" => "✌",
				"✌" => "✌",
				"グー" => "✊",
				"ぐー" => "✊",
				"✊" => "✊",
				"👊" => "✊",
				"パー" => "✋",
				"ぱー" => "✋",
				"✋" => "✋",
				"🤚" => "✋",
				"🖐" => "✋",
				_ => null,
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
			"$user$強いな．またやりましょ",
			"楽しかった．ありがとう！",
			"悔しい... 次は負けないよ．",
			"うーむ... 次こそは",
			"うーん，$user$...，泣きの一回はダメですか😢"
		};

		private readonly string[] winMessage = {
			"楽しかった. ありがとう!",
			"$user$, 落ち込まないで...またやろ?",
			"またやりましょう👍"
		};

		private enum Result
		{
			Win,
			Draw,
			Lose,
		}
	}
}
