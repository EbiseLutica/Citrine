using System.Collections.Generic;
using System.Linq;
using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiPoll : IPoll
	{
		readonly Poll poll;
		public MiPoll(Poll p)
		{
			poll = p;
			// 一括変換
			Choices = from c in p.Choices select new MiChoice(c);
		}
		public IEnumerable<IChoice> Choices { get; }
	}


}
