#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class FollowBackModule : ModuleBase
	{
		public override async Task<bool> OnFollowedAsync(IUser user, IShell shell, Server core)
		{
			if (core.GetRatingOf(user) == Rating.Hate)
				return true;
			core.LikeWithLimited(user);
			await shell.FollowAsync(user);
			return false;
		}
	}
}
