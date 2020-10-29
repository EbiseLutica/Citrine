#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class DumpCommand : CommandBase
	{
		public override string Name => "dump";

		public override string Usage => "/dump";

		public override string Description => "このコマンドを含む投稿の情報を、アプリケーションの標準入出力に表示します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (sender is not PostCommandSender p)
				return "このコマンドはユーザーが実行してください.";
			var n = p.Post;

			logger.Info($@"Dumped Post
id: {n.Id}
name: {n.User.Name ?? "NULL"}
host: {n.User.Host ?? "NULL"}
screenName: {n.User.ScreenName ?? "NULL"}
text: {n.Text ?? "NULL"}
visibility: {n.Visiblity}");
			return "この投稿をコンソールに出力しました. コンソール画面を確認してください.";
		}

		private readonly Logger logger = new Logger(nameof(DumpCommand));
	}
}
