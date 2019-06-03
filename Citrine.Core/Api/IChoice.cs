namespace Citrine.Core.Api
{
	public interface IChoice
	{
		int Id { get; }
		string Text { get; }
		long Count { get; }
	}
}
