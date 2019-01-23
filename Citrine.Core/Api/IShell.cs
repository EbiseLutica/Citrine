using System;
using System.Threading.Tasks;

namespace Citrine.Core.Api
{
	public interface IShell
	{
		IUser Myself { get; }

		Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default);
		Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default);
		Task ReactAsync(IPost post, string reactionChar);
		Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default);
		Task<IPost> SendDirectMessageAsync(IUser user, string text);
		Task VoteAsync(IPost post, int choice);
	}
}
