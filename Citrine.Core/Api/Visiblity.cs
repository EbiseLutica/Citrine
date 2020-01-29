namespace Citrine.Core.Api
{
	/// <summary>
	/// 投稿の公開範囲を指定します。
	/// </summary>
	public enum Visibility
	{
		/// <summary>
		/// デフォルト。多くの場合元の投稿を継承します。
		/// </summary>
		Default,
		/// <summary>
		/// 公開。
		/// </summary>
		Public,
		/// <summary>
		/// 未収載。
		/// </summary>
		Limited,
		/// <summary>
		/// フォロワー限定。
		/// </summary>
		Private,
		/// <summary>
		/// ダイレクト。
		/// </summary>
		Direct,
	}
}
