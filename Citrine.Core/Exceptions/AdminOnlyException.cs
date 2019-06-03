namespace Citrine.Core
{
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
}
