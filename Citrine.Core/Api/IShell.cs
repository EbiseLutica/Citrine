using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Citrine.Core.Api
{
	public interface IShell
	{
		IUser Myself { get; }
		bool CanCreatePoll { get; }
		bool CanBlock { get; }
		bool CanMute { get; }
		bool CanFollow { get; }
		AttachmentType AttachmentType { get; }
		int AttachmentMaxCount { get; }

		Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<IAttachment> attachments = null);
		Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<string> filePaths = null);
		Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<IAttachment> attachments = null);
		Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<string> filePaths = null);
		Task ReactAsync(IPost post, string reactionChar);
		Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default);
		Task<IPost> SendDirectMessageAsync(IUser user, string text);
		Task<IAttachment> UploadAsync(string path, string name);
		Task DeleteFileAsync(IAttachment attachment);
		Task FollowAsync(IUser user);
		Task UnfollowAsync(IUser user);
		Task BlockAsync(IUser user);
		Task UnBlockAsync(IUser user);
		Task MuteAsync(IUser user);
		Task UnMuteAsync(IUser user);
		Task VoteAsync(IPost post, int choice);
		Task LikeAsync(IPost post);
		Task UnlikeAsync(IPost post);

		Task<IPost> GetPostAsync(string id);
		Task<IUser> GetUserAsync(string id);
		Task<IUser> GetUserByNameAsync(string name);
		Task<IAttachment> GetAttachmentAsync(string fileId);
	}

	public enum AttachmentType
	{
		Unsupported,
		BindToThePost,
		UploadAndAttach,
	}
}
