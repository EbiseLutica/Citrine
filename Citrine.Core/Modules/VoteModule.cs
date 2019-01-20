using System;
using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class VoteModule : ModuleBase
	{
		Random rnd = new Random();
		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			if (n == null)
				return false;

			await Task.Delay(1000);

			// ランダムで投票する
			await shell.Notes.Polls.VoteAsync(n.Id, rnd.Next(n.Poll.Choices.Count()));

			// 多分競合しないから常にfalse
			return false;
		}
	}
}
