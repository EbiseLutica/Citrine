using System;

namespace Citrine.Core.Api
{
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
