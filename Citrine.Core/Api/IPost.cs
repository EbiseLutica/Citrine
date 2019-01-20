namespace Citrine.Core.Api
{
	public interface IPost
	{
		string Id { get; }
		IUser User { get; }
		string Text { get; }
		bool IsRepost { get; }
		IPost Repost { get; }
		bool IsReply { get; }
		IPost Reply { get; }
		bool RepostCount { get; }
		IPoll Poll { get; }
	}
}
