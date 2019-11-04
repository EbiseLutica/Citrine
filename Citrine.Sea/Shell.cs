using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;

namespace Citrine.Sea
{
    public class Shell : IShell
    {
        public static string Version => "1.0.0";

        public IUser? Myself { get; private set; }

		public Server? Core { get; private set; }

        public bool CanCreatePoll => false;

        public bool CanBlock => false;

        public bool CanMute => false;

        public bool CanFollow => false;

        public AttachmentType AttachmentType => AttachmentType.UploadAndAttach;

        public int AttachmentMaxCount => int.MaxValue;

        public static async Task<Shell> InitializeAsync()
        {
			var s = new Shell();
			s.Core = new Server(s);
			return s;
        }

        public Task BlockAsync(IUser user)
        {
            throw new NotSupportedException();
        }

        public Task DeleteFileAsync(IAttachment attachment)
        {
            throw new NotSupportedException();
        }

        public Task DeletePostAsync(IPost post)
        {
            throw new NotSupportedException();
        }

        public Task FollowAsync(IUser user)
        {
            throw new NotSupportedException();
        }

        public Task<IAttachment> GetAttachmentAsync(string fileId)
        {
            throw new NotImplementedException();
        }

        public Task<IPost> GetPostAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IUser> GetUserAsync(string id)
        {
			throw new NotSupportedException();
        }

        public Task<IUser> GetUserByNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        public Task LikeAsync(IPost post)
        {
			return ReactAsync(post, "❤️");
        }

        public Task MuteAsync(IUser user)
        {
            throw new NotSupportedException();
        }

        public Task<IPost> PostAsync(string text, string? cw = null, Visiblity visiblity = Visiblity.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
        {
            throw new NotImplementedException();
        }

        public Task<IPost> PostWithFilesAsync(string text, string? cw = null, Visiblity visiblity = Visiblity.Default, List<string>? choices = null, params string[] filePaths)
        {
            throw new NotImplementedException();
        }

        public Task ReactAsync(IPost post, string reactionChar)
        {
			return PostAsync($"@{post.User.Name} {reactionChar}");
        }

        public Task<IPost> ReplyAsync(IPost post, string text, string? cw = null, Visiblity visiblity = Visiblity.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
        {
            throw new NotImplementedException();
        }

        public Task<IPost> ReplyWithFilesAsync(IPost post, string text, string? cw = null, Visiblity visiblity = Visiblity.Default, List<string>? choices = null, List<string>? filePaths = null)
        {
            throw new NotImplementedException();
        }

        public Task<IPost> RepostAsync(IPost post, string? text = null, string? cw = null, Visiblity visiblity = Visiblity.Default)
        {
			return PostAsync($"RP @{post.User.Name}: {post.Text}");
        }

        public Task<IPost> SendDirectMessageAsync(IUser user, string text)
        {
			throw new NotSupportedException();
        }

        public Task UnblockAsync(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task UnfollowAsync(IUser user)
        {
			throw new NotSupportedException();
        }

        public async Task UnlikeAsync(IPost post)
        {
			// do nothing
        }

        public async Task UnmuteAsync(IUser user)
        {
			// do nothing
        }

        public Task<IAttachment> UploadAsync(string path, string name)
        {
            throw new NotImplementedException();
        }

        public Task VoteAsync(IPost post, int choice)
        {
            throw new NotSupportedException();
        }
    }
}
