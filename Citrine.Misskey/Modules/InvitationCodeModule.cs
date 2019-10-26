using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;

namespace Citrine.Misskey
{
	public class InvitationCodeModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var s = shell as Shell;
			var mk = s.Misskey;
			if (n.Text.Contains("招待"))
			{
				if (core.Storage[n.User].Get<string>("invitation-code") is string code)
				{
					await shell.ReplyAsync(n, $"あれ, あなたには既にコードをお渡ししていますよ. 招待コードは **{code}** です.", null, Visiblity.Direct);
				}

				var meta = await mk.MetaAsync();

				var (left, right, op, answer) = GenerateQuiz();

				var ctx = await shell.ReplyAsync(n, $@"**{meta.Name ?? "本サーバー"}** への参加をご検討頂き, ありがとうございます!
これから招待コードを発行します. その前にスパム対策の為, 簡単なクイズにお答えください.

{left} {op} {right} の答えは？", null, Visiblity.Direct);
				core.RegisterContext(ctx, this, new Dictionary<string, object>
				{
					{ "result", answer.ToString() },
				});
				return true;
			}
			return false;
		}

		public override async Task<bool> OnRepliedContextually(IPost n, IPost context, Dictionary<string, object> store, IShell shell, Server core)
		{
			var s = shell as Shell;
			var mk = s.Misskey;
			if (n.Text != null)
			{
				if (n.Text.TrimMentions() == store["result"].ToString())
				{
					var code = (await mk.Admin.InviteAsync()).Code;
					await shell.ReplyAsync(n, $"正解です! 招待コードは **{code}**です.", null, Visiblity.Direct);

					core.Storage[n.User].Set("invitation-code", code);
				}
				else
				{
					var ctx = await shell.ReplyAsync(n, "不正解です. もう一度答えてください!", null, Visiblity.Direct);
					core.RegisterContext(ctx, this, store);
				}
				return true;
			}
			return false;
		}

		private (int, int, string, int) GenerateQuiz()
		{
			var left = rnd.Next(1, 10);
			var right = rnd.Next(1, 10);
			var op = "+-×"[rnd.Next(3)].ToString();
			var ans = op switch
			{
				"+" => left + right,
				"-" => left - right,
				"×" => left * right,
				_ => 0,
			};
			return (left, right, op, ans);
		}

		readonly Random rnd = new Random();
	}
}
