namespace Citrine.Core
{
	/// <summary>
	/// コマンドの実行に失敗した際にスローされます。
	/// </summary>
	[System.Serializable]
	public class CommandException : System.Exception
	{
		public CommandException() { }
		public CommandException(string message) : base(message) { }
		public CommandException(string message, System.Exception inner) : base(message, inner) { }
		protected CommandException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
