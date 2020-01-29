using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Citrine.Core.Api
{
	public interface IShell
	{
		/// <summary>
		/// bot ユーザーを取得します。
		/// </summary>
		IUser Myself { get; }

		/// <summary>
		/// 投票を作れるかどうかを示す値を取得します。
		/// </summary>
		bool CanCreatePoll { get; }

		/// <summary>
		/// ブロック機能を持つかどうかを示す値を取得します。
		/// </summary>
		bool CanBlock { get; }

		/// <summary>
		/// ミュート機能を持つかどうかを示す値を取得します。
		/// </summary>
		bool CanMute { get; }

		/// <summary>
		/// ユーザーをフォローできるかどうかを示す値を取得します。
		/// </summary>
		bool CanFollow { get; }

		/// <summary>
		/// 添付ファイルの扱われ方を取得します。
		/// </summary>
		AttachmentType AttachmentType { get; }

		/// <summary>
		/// 投稿に添付可能なファイル数を取得します。
		/// </summary>
		int AttachmentMaxCount { get; }

		/// <summary>
		/// 指定した投稿に対しリプライします。
		/// </summary>
		/// <param name="post">リプライ先の投稿。</param>
		/// <param name="text">テキスト。</param>
		/// <param name="cw">警告文。</param>
		/// <param name="visiblity">公開範囲。</param>
		/// <param name="choices">投票の選択肢。</param>
		/// <param name="attachments">添付ファイル。</param>
		/// <returns>作成された投稿。</returns>
		Task<IPost> ReplyAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null);

		/// <summary>
		/// 指定した投稿に対しファイルをアップロードし、添付してリプライします。
		/// </summary>
		/// <param name="post">リプライ先の投稿。</param>
		/// <param name="text">テキスト。</param>
		/// <param name="cw">警告文。</param>
		/// <param name="visiblity">公開範囲。</param>
		/// <param name="choices">投票の選択肢。</param>
		/// <param name="filePaths">ファイルパス。</param>
		/// <returns>作成された投稿。</returns>
		Task<IPost> ReplyWithFilesAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<string>? filePaths = null);

		/// <summary>
		/// 投稿を作成します。
		/// </summary>
		/// <param name="text">テキスト。</param>
		/// <param name="cw">警告文。</param>
		/// <param name="visiblity">公開範囲。</param>
		/// <param name="choices">投票の選択肢。</param>
		/// <param name="attachments">添付ファイル。</param>
		/// <returns>作成された投稿。</returns>
		Task<IPost> PostAsync(string text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null);

		/// <summary>
		/// ファイルをアップロードし、投稿に添付し送信します。
		/// </summary>
		/// <param name="text">テキスト。</param>
		/// <param name="cw">警告文。</param>
		/// <param name="visiblity">公開範囲。</param>
		/// <param name="choices">投票の選択肢。</param>
		/// <param name="attachments">添付ファイル。</param>
		/// <returns>作成された投稿。</returns>
		Task<IPost> PostWithFilesAsync(string text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, params string[] filePaths);

		/// <summary>
		/// 指定した投稿にリアクションを送ります。リアクションをサポートしない環境では代替となる挙動を示します。例: いいね
		/// </summary>
		/// <param name="post">指定した投稿</param>
		/// <param name="reactionChar">リアクションに使う絵文字。</param>
		Task ReactAsync(IPost post, string reactionChar);

		/// <summary>
		/// 指定した投稿をリポストします。
		/// </summary>
		/// <param name="post">リポストする投稿。</param>
		/// <param name="text">テキスト。指定した場合引用リポストされますが、非サポート環境では代替の挙動を示します。</param>
		/// <param name="cw">警告文。</param>
		/// <param name="visiblity">公開範囲。</param>
		/// <returns>作成した投稿。</returns>
		Task<IPost> RepostAsync(IPost post, string? text = null, string? cw = null, Visibility visiblity = Visibility.Default);

		/// <summary>
		/// 指定したユーザーにダイレクトメッセージを送信します。
		/// </summary>
		/// <param name="user">ユーザー。</param>
		/// <param name="text">テキスト。</param>
		/// <returns>作成したメッセージ。</returns>
		Task<IPost> SendDirectMessageAsync(IUser user, string text);

		/// <summary>
		/// ファイルをアップロードします。
		/// </summary>
		/// <param name="path">ファイルパス。</param>
		/// <param name="name">ファイル名。</param>
		/// <returns>作成したファイル。</returns>
		Task<IAttachment> UploadAsync(string path, string name);

		/// <summary>
		/// ファイルを削除します。
		/// </summary>
		/// <param name="attachment">削除するファイル。</param>
		Task DeleteFileAsync(IAttachment attachment);

		/// <summary>
		/// 指定したユーザーをフォローします。
		/// </summary>
		Task FollowAsync(IUser user);

		/// <summary>
		/// 指定したユーザーのフォローを解除します。
		/// </summary>
		Task UnfollowAsync(IUser user);

		/// <summary>
		/// 指定したユーザーをブロックします。
		/// </summary>
		Task BlockAsync(IUser user);

		/// <summary>
		/// 指定したユーザーのブロックを解除します。
		/// </summary>
		Task UnblockAsync(IUser user);

		/// <summary>
		/// 指定したユーザーをミュートします。
		/// </summary>
		Task MuteAsync(IUser user);

		/// <summary>
		/// 指定したユーザーのミュートを解除します。
		/// </summary>
		Task UnmuteAsync(IUser user);

		/// <summary>
		/// 投稿を削除します。
		/// </summary>
		Task DeletePostAsync(IPost post);

		/// <summary>
		/// 指定した投稿に投票します。
		/// </summary>
		Task VoteAsync(IPost post, int choice);

		/// <summary>
		/// 指定した投稿をいいね！します。
		/// </summary>
		Task LikeAsync(IPost post);

		/// <summary>
		/// 指定した投稿のいいね！を解除します。
		/// </summary>
		Task UnlikeAsync(IPost post);

		/// <summary>
		/// ID から投稿を取得します。
		/// </summary>
		/// <returns>取得した投稿。</returns>
		Task<IPost> GetPostAsync(string id);
		/// <summary>
		/// ID からユーザーを取得します。
		/// </summary>
		/// <returns>取得したユーザー。</returns>
		Task<IUser> GetUserAsync(string id);

		/// <summary>
		/// ユーザー名からユーザーを取得します。
		/// </summary>
		/// <returns>取得したユーザー。</returns>
		Task<IUser> GetUserByNameAsync(string name);

		/// <summary>
		/// ID からファイルを取得します。
		/// </summary>
		/// <param name="fileId"></param>
		/// <returns>取得したファイル。</returns>
		Task<IAttachment> GetAttachmentAsync(string fileId);
	}

	/// <summary>
	/// 添付ファイルの取り扱い方を指定します。
	/// </summary>
	public enum AttachmentType
	{
		/// <summary>
		/// ファイル添付はサポートされません。
		/// </summary>
		Unsupported,
		/// <summary>
		/// ファイルは投稿に必ず紐付けられます。
		/// </summary>
		BindToThePost,
		/// <summary>
		/// ファイルはクラウドドライブのような領域に配置され、それを投稿に紐づけてシェアします。
		/// </summary>
		UploadAndAttach,
	}
}
