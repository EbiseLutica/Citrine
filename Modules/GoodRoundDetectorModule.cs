using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	/// <summary>
	/// キリ番ノート検出モジュール
	/// </summary>
	public class GoodRoundDetectorModule : ModuleBase
	{
		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (core.GetRatingOf(n.User) < Rating.Like) return false;
			// 投稿についているユーザー名は完璧では無い
			// なのでシェルを用いて完全体を持ってくる
			var user = await shell.GetUserAsync(n.User.Id);
			if (user == null) return false;

			if (user.PostsCount > 0 && user.PostsCount % 10000 == 0)
			{
				await shell.SendDirectMessageAsync(user, $"{core.GetNicknameOf(user)}，{user.PostsCount}投稿達成おめでとう．");
			}

			return false;
		}
	}
}
