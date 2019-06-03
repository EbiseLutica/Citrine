#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
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
}
