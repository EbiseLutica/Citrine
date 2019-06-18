#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class FollowBackModule : ModuleBase
	{
		public override async Task<bool> OnFollowedAsync(IUser user, IShell shell, Server core)
		{
			await shell.FollowAsync(user);
			return false;
		}
	}
}
