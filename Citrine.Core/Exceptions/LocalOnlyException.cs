namespace Citrine.Core
{
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

	[System.Serializable]
	public class RemoteOnlyException : System.Exception
	{
		public RemoteOnlyException() { }
		public RemoteOnlyException(string message) : base(message) { }
		public RemoteOnlyException(string message, System.Exception inner) : base(message, inner) { }
		protected RemoteOnlyException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
