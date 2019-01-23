namespace Citrine.Core.Api
{
	public interface IUser
	{
		string Name { get; }
		string IconUrl { get; }
		string ScreenName { get; }
		string Id { get; }
		string Description { get; }
		string Host { get; }
		bool IsVerified { get; }
		bool IsBot { get; }
		long FollowingsCount { get; }
		long FollowersCount { get; }
		long PostsCount { get; }
	}
}
