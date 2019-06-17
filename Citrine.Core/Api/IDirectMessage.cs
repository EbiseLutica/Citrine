namespace Citrine.Core.Api
{
	public interface IDirectMessage : IPost
	{
		IUser Recipient { get; }
		bool IsRead { get; }
	}
}
