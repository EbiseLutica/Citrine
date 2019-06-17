namespace Citrine.Core.Api
{
	public interface IDirectMessage
	{
		IUser Recipient { get; }
		bool IsRead { get; }
	}
}
