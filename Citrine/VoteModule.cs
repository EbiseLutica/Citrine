using System;
using System.Linq;
using System.Threading.Tasks;
using Disboard.Misskey;
using Disboard.Misskey.Models;

namespace Citrine
{
	public class VoteModule : ModuleBase
	{
		Random rnd = new Random();
		public override async Task<bool> OnTimelineAsync(Note n, MisskeyClient mi, Citrine core)
		{
			if (n.Poll == null)
				return false;

			await Task.Delay(1000);

			// ランダムで投票する
			await mi.Notes.Polls.VoteAsync(n.Id, rnd.Next(n.Poll.Choices.Count()));

			// 多分競合しないから常にfalse
			return false;
		}
	}
}
