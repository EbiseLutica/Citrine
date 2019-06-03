#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
    public class EchoCommand : CommandBase
	{
		public override string Name => "echo";

		public override string Usage => "/echo <text>";

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			return body;
		}
	}

    public class VersionCommand : CommandBase
	{
		public override string Name => "version";

		public override string Usage => "/version or /ver or /v";

		public override string[] Aliases { get; } = { "ver", "v" };

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			return $"Citrine v{Server.Version} / XelticaBot v{Server.VersionAsXelticaBot}";
		}
	}

    public class UserAgentCommand : CommandBase
	{
		public override string Name => "useragent";

		public override string Usage => "/useragent or /ua";

		public override string[] Aliases { get; } = { "ua" };

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			return Server.Http.DefaultRequestHeaders.UserAgent.ToString();
		}
	}

    public class DumpCommand : CommandBase
	{
		public override string Name => "dump";

		public override string Usage => "/dump";

		public override async Task<string> OnActivatedAsync(IPost n, Server core, IShell shell, string[] args, string body)
		{
				Console.WriteLine($@"Dumped Post
id: {n.Id}
name: {n.User.Name ?? "NULL"}
screenName: {n.User.ScreenName ?? "NULL"}
text: {n.Text ?? "NULL"}
visibility: {n.Visiblity}");
				return "この投稿をコンソールに出力しました. コンソール画面を確認してください.";
		}
	}
}
