using System;
using Disboard.Mastodon;
using Disboard.Mastodon.Enums;

namespace Citrine.Mastodon
{
	class Program
	{
		static void Main(string[] args)
		{
			var cli = new MastodonClient(Domain);
			var scope = AccessScope.Read | AccessScope.Write | AccessScope.Follow;
			
		}

		private static readonly string Domain = "botdon.net";
	}
}
