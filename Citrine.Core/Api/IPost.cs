using System.Collections.Generic;

namespace Citrine.Core.Api
{
	/// <summary>
	/// 投稿を定義します。
	/// </summary>
	public interface IPost
	{
		/// <summary>
		/// 投稿の ID を取得します。
		/// </summary>
		string Id { get; }
		/// <summary>
		/// 投稿元のユーザーを取得します。
		/// </summary>
		IUser User { get; }
		/// <summary>
		/// 投稿のテキストを取得します。
		/// </summary>
		string Text { get; }
		/// <summary>
		/// 投稿がリポストであるかどうかを示す値を取得します。
		/// </summary>
		bool IsRepost { get; }
		/// <summary>
		/// リポスト元の投稿を取得します。
		/// </summary>
		IPost Repost { get; }
		/// <summary>
		/// 投稿がリプライであるかどうかを示す値を取得します。
		/// </summary>
		bool IsReply { get; }
		/// <summary>
		/// リプライ元の投稿を取得します。
		/// </summary>
		IPost Reply { get; }
		/// <summary>
		/// リポストの数を取得します。
		/// </summary>
		long RepostCount { get; }
		/// <summary>
		/// 投稿の公開範囲を取得します。
		/// </summary>
		Visiblity Visiblity { get; }
		string NativeVisiblity { get; }
		/// <summary>
		/// 投稿に使用されたクライアント名を取得します。
		/// </summary>
		string Via { get; }
		/// <summary>
		/// 投稿に付属する投票を取得します。
		/// </summary>
		IPoll Poll { get; }
		/// <summary>
		/// 投票の添付ファイルを取得します。
		/// </summary>
		List<IAttachment> Attachments { get; }
	}
}
