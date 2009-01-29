namespace IrisIM
{
	namespace Utilities
	{
		public class FailedConnection : IrisIMException
		{
			public FailedConnection(string message) : base(message)
			{}
		}
	}
}