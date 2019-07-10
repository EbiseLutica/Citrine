using System;
using Citrine.Core.Api;

namespace Citrine.Core
{
    public static class ServerExtension
	{
		public static void LikeWithLimited(this Server core, IUser user, int incrementation = 1)
		{
			var span = DateTime.Now - new DateTime(core.Storage[user].Get(StorageKey.LastPlayingDate, 0L));
			if (span < new TimeSpan(0, 0, 30))
				return;
			core.Like(user.Id, incrementation);
			core.Storage[user].Set(StorageKey.LastPlayingDate, DateTime.Now.Ticks);
		}

		public static void OnHarassment(this Server core, IUser user, int decrementation = 1)
		{
			core.Dislike(user.Id, decrementation);
			core.Storage[user].Set(StorageKey.HarrasmentedCount, core.Storage[user].Get(StorageKey.HarrasmentedCount, 0) + decrementation);
		}

		public static bool TryApologyze(this Server core, IUser user, int incrementation = 1)
		{
			if (core.Storage[user].Get(StorageKey.HarrasmentedCount, 0) < 3)
			{
				core.Like(user.Id, incrementation);
				return true;
			}
			else
				return false;
		}
	}

}