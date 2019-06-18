using System;

namespace Citrine.Core.Api
{
	/// <summary>
	/// 投稿に付属する添付を定義します。
	/// </summary>
	public interface IAttachment
	{
		string Id { get; }
		string Name { get; }
		string Url { get; }
		string PreviewUrl { get; }
		DateTime CreatedAt { get; }
		string Comment { get; }
	}
}
