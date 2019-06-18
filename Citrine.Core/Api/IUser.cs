namespace Citrine.Core.Api
{
	/// <summary>
	/// ユーザーを定義します。
	/// </summary>
	public interface IUser
	{
		/// <summary>
		/// ユーザー名を取得します。
		/// </summary>
		string Name { get; }

		/// <summary>
		/// アイコンの URL を取得します。
		/// </summary>
		string IconUrl { get; }

		/// <summary>
		/// 表示名を取得します。
		/// </summary>
		string ScreenName { get; }

		/// <summary>
		/// ID を取得します。
		/// </summary>
		string Id { get; }

		/// <summary>
		/// ユーザーの説明を取得します。
		/// </summary>
		string Description { get; }

		/// <summary>
		/// ユーザーの所属するホストを取得します。
		/// </summary>
		string Host { get; }

		/// <summary>
		/// ユーザーが認証済みであるかどうか示す値を取得します。
		/// </summary>
		bool IsVerified { get; }

		/// <summary>
		/// ユーザーが bot であるかどうか示す値を取得します。
		/// </summary>
		bool IsBot { get; }

		/// <summary>
		/// ユーザーのフォロー数を取得します。
		/// </summary>
		long FollowingsCount { get; }

		/// <summary>
		/// ユーザーのフォロワー数を取得します。
		/// </summary>
		long FollowersCount { get; }

		/// <summary>
		/// ユーザーの投稿数を取得します。
		/// </summary>
		long PostsCount { get; }
	}
}
