namespace Citrine.Core
{
	public class SuperInternalCommandSender : ICommandSender
	{
		public bool IsAdmin => true;

		public static SuperInternalCommandSender Instance { get; } = new SuperInternalCommandSender();
	}
}
