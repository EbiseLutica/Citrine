using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class UserMovingKit : ModuleBase, ICommand
	{
		public string Name => "moving-code";

		public string[] Aliases => new string[0];

		public bool IgnoreCase => true;

		public PermissionFlag Permission => PermissionFlag.Any;

		public string Usage => "/moving-code <your moving code>";

		public string Description => "引っ越しコードを入力して、他アカウントからのお引越しを行います。";

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;

			string? message = null;

			if (n.Text.IsMatch("(引っ?越|ひっこ)しし?たい"))
			{
				if (movingCodesSet.FirstOrDefault(m => m.userId == n.User.Id) is (string userId, string movingCode) set)
				{
					// 既にコードがあればそれを返す
					message = $"ん, あなたには既に引っ越し手続きコードを発行しているね. 引っ越ししたいアカウントで私に、リプライで次のようなコマンドを送ってね.\n/moving-code {set.movingCode}";
				}
				else
				{
					// 10桁のIDを発行
					var code = GenerateCode(10);

					while (movingCodesSet.Any(c => c.movingCode == code))
					{
						// コードが被ってしまったら再生成する
						code = GenerateCode(10);
					}
					movingCodesSet.Add((n.User.Id, code));
					message = $"はい, 引っ越し手続きコードを発行したよ. 引っ越ししたいアカウントで私に、リプライで次のようなコマンドを送ってね.\n/moving-code {code}";
				}
			}
			else if (n.Text.IsMatch("(引っ?越|ひっこ)しキャンセル"))
			{
				if (!(movingCodesSet.FirstOrDefault(m => m.userId == n.User.Id) is (string userId, string movingCode) set))
				{
					message = "ん...? あなたは元々引っ越し手続きをしていないみたいだよ? 引っ越しをしたかったら, 引っ越ししたい って言ってね.";
				}
				else
				{
					movingCodesSet.RemoveAll(r => r.userId == n.User.Id);
					message = "わかった. 引っ越し手続きはキャンセルしたよ. またしたくなったら声かけてね.";
				}
			}
			if (message != null)
			{
				await shell.ReplyAsync(n, message, null, Visibility.Direct);
				return true;
			}
			return false;
		}

		public async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (body.ToLowerInvariant() == "dump")
			{
				if (!sender.IsAdmin)
					return "admin only";
				var dumped = string.Join("\n", movingCodesSet.Select(s => $"{s.userId}: {s.movingCode}"));
				return string.IsNullOrEmpty(dumped) ? "no set" : dumped;
			}
			if (sender is not PostCommandSender s)
			{
				return "ユーザーから実行してください.";
			}

			if (!(movingCodesSet.FirstOrDefault(m => m.movingCode == body.Trim()) is (string userId, string movingCode) set))
			{
				return "その引っ越し手続きコードは存在しないよ. コードが正しいかもう一度確認してね.";
			}

			core.Storage.Migrate(set.userId, s.User.Id);
			return "引っ越しが完了したよ〜. これからもよろしくね.";
		}

		private string GenerateCode(int length)
		{
			const string c = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat('\0', length).Select(_ => c.Random()).ToArray());
		}

		private readonly List<(string userId, string movingCode)> movingCodesSet = new List<(string userId, string movingCode)>();
	}

}
