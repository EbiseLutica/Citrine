using Citrine.Core.Api;

namespace Citrine.Core
{
	public class PostCommandSender : ICommandSender
	{
		public IPost Post { get; }

		public IUser User => Post.User;

		public bool IsAdmin { get; }

		public PostCommandSender(IPost post, bool isAdmin)
		{
			Post = post;
			IsAdmin = isAdmin;
		}
	}
}
