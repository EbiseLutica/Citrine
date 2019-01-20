using System;
using System.Threading.Tasks;

namespace Citrine.Core.Api
{
	public interface IShell
	{
		bool CanFollow { get; }

		Task<IPost> Reply(IPost post, string text, string cw, Visiblity visiblity = Visiblity.Default);
		Task<IPost> Post(string text, string cw, Visiblity visiblity = Visiblity.Default);
		Task React(IPost post, string reactionChar);
		Task<IPost> Repost(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default);
		Task<IPost> SendDirectMessage(IUser user, string text, Visiblity visiblity = Visiblity.Default);
	}
}
