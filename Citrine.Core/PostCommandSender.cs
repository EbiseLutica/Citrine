using Citrine.Core.Api;

namespace Citrine.Core
{
    public class PostCommandSender : ICommandSender
    {
		public IPost Post { get; }

		public IUser User => Post?.User;

		public PostCommandSender(IPost post)
		{
			Post = post;
		}
    }
}
