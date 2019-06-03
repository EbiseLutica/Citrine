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

	[System.Serializable]
	public class AdminOnlyException : System.Exception
	{
		public AdminOnlyException() { }
		public AdminOnlyException(string message) : base(message) { }
		public AdminOnlyException(string message, System.Exception inner) : base(message, inner) { }
		protected AdminOnlyException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[System.Serializable]
	public class LocalOnlyException : System.Exception
	{
		public LocalOnlyException() { }
		public LocalOnlyException(string message) : base(message) { }
		public LocalOnlyException(string message, System.Exception inner) : base(message, inner) { }
		protected LocalOnlyException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
