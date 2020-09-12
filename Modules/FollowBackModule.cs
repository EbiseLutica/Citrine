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

        public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
        {
            if (n.Text == null) return false;
            if (n.Text.IsMatch("フォロ[ーバ](バック)?し"))
            {
                if (core.GetRatingOf(n.User) == Rating.Hate)
                    return true;
                core.LikeWithLimited(n.User);
                await shell.FollowAsync(n.User);
                await shell.ReactAsync(n, "✌️");
                return true;
            }
            if (n.Text.IsMatch("フォロ[ーバ](バック)?(解除|外し|[や辞]め)"))
            {
                core.LikeWithLimited(n.User);
                await shell.UnfollowAsync(n.User);
                await shell.ReactAsync(n, "👋");
                return true;
            }
            return false;
        }
	}
}
