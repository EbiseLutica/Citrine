namespace Citrine.Core.Api
{
	/// <summary>
	/// 投票の選択肢を定義します。
	/// </summary>
	public interface IChoice
	{
		/// <summary>
		/// 選択肢の番号を取得します。
		/// </summary>
		int Id { get; }
		/// <summary>
		/// 選択肢のテキストを取得します。
		/// </summary>
		string Text { get; }
		/// <summary>
		/// この選択肢の投票数を取得します。
		/// </summary>
		long Count { get; }
	}
}
