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
		long RepostCount { get; }
		Visiblity Visiblity { get; }
		string NativeVisiblity { get; }
		string Via { get; }
		IPoll Poll { get; }
	}
}
