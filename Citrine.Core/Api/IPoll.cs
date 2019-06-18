using System.Collections.Generic;

namespace Citrine.Core.Api
{
	/// <summary>
	/// 投票を定義します。
	/// </summary>
	public interface IPoll
	{
		/// <summary>
		/// 選択肢の一覧を取得します。
		/// </summary>
		IEnumerable<IChoice> Choices { get; }
	}
}
