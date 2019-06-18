namespace Citrine.Core.Api
{
	/// <summary>
	/// ダイレクトメッセージ形式のポストを定義します。
	/// </summary>
	public interface IDirectMessage : IPost
	{
		/// <summary>
		/// 送り先を取得します。
		/// </summary>
		IUser Recipient { get; }
		/// <summary>
		/// 既読済みであるかどうかを取得します。
		/// </summary>
		bool IsRead { get; }
	}
}
