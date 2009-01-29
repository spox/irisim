namespace IrisIM
{
	namespace Utilities
	{
		public class FatalException : IrisIMException
		{
			public FatalException(string message) : base(message)
			{}
		}
	}
}