
namespace IrisIM
{
	namespace Utilities
	{
		public class KeyExists : IrisIMException
		{
			public KeyExists() : base()
			{}
			
			public KeyExists(string message) : base(message)
			{}
		}
	}
}