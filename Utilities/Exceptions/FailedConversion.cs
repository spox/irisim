namespace IrisIM
{
	namespace Utilities
	{
		public class FailedConversion : IrisIMException
		{
			public FailedConversion(string message) : base(message)
			{}
		}
	}
}