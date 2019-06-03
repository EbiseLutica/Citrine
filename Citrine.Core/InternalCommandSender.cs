namespace Citrine.Core
{
	public class InternalCommandSender : ICommandSender
	{
		public bool IsAdmin => false;
		public static InternalCommandSender Instance { get; } = new InternalCommandSender();
	}
}
