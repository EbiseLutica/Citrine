using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiDmPost : IPost
	{
		public Message Native { get; }
		public MiDmPost(Message mes)
		{
			Native = mes;

			Id = mes.Id;
			User = new MiUser(mes.User);
			Text = mes.Text;
		}

		public string Id { get; }

		public IUser User { get; }

		public string Text { get; }

		#region unsupported props
		public bool IsRepost => false;

		public IPost Repost => default;

		public bool IsReply => false;

		public IPost Reply => default;

		public long RepostCount => 0;

		public Visiblity Visiblity => Visiblity.Default;

		public string NativeVisiblity => default;

		public string Via => default;

		public IPoll Poll => default;
		#endregion
	}
}
