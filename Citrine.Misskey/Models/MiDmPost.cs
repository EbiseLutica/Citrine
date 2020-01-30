using System.Collections.Generic;
using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiDmPost : IDirectMessage
	{
		public Message Native { get; }
		public MiDmPost(Message mes)
		{
			Native = mes;

			Id = mes.Id;
			User = new MiUser(mes.User);
			Text = mes.Text;
			IsRead = mes.IsRead;
			Recipient = new MiUser(mes.Recipient);
			Attachments = mes.File != null ? new List<IAttachment> { new MiAttachment(mes.File) } : null;
		}

		public string Id { get; }

		public IUser User { get; }

		public string Text { get; }

		public IUser Recipient { get; }

		public bool IsRead { get; }

		public List<IAttachment> Attachments { get; }

		#region unsupported props
		public bool IsRepost => false;

		public IPost Repost => default;

		public bool IsReply => false;

		public IPost Reply => default;

		public long RepostCount => 0;

		public Visibility Visiblity => Visibility.Default;

		public string NativeVisiblity => default;

		public string Via => default;

		public IPoll Poll => default;
		#endregion
	}
}
