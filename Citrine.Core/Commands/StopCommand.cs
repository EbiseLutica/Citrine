using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class StopCommand : CommandBase
	{
		public override string Name => "stop";

		public override string Usage => "/stop";

		public override PermissionFlag Permission => PermissionFlag.AdminOnly;

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			await shell.ReplyAsync(source, "またねー。");
			return null;
		}
	}
}
