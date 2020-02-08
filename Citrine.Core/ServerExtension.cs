using System;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public static class ServerExtension
	{
		/// <summary>
		/// リミテーション付きで好感度を上げます。30秒以内に指定したユーザーの好感度を既に上げている場合は何もしません。
		/// </summary>
		/// <param name="user"></param>
		/// <param name="incrementation"></param>
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

		public static UserStorage.UserRecord GetMyStorage(this Server core) => core.Storage[core.Shell.Myself!];
	}

}
