namespace Citrine.Core.Api
{
	public interface IUser
	{
		string Name { get; }
		string IconUrl { get; }
		string ScreenName { get; }
		string InternalId { get; }
		string Description { get; }
		bool IsVerified { get; }
		bool IsBot { get; }
		string Via { get; }
		int FollowingsCount { get; }
		int FollowersCount { get; }
		int PostsCount { get; }
	}
}
