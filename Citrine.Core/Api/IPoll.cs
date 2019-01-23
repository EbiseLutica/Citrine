using System.Collections.Generic;

namespace Citrine.Core.Api
{
	public interface IPoll
	{
		IEnumerable<IChoice> Choices { get; }
	}
}
