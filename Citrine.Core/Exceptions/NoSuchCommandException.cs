namespace Citrine.Core
{
    [System.Serializable]
	public class NoSuchCommandException : System.Exception
	{
		public NoSuchCommandException() { }
		public NoSuchCommandException(string message) : base(message) { }
		public NoSuchCommandException(string message, System.Exception inner) : base(message, inner) { }
		protected NoSuchCommandException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
