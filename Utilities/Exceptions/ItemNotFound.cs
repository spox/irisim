namespace IrisIM
{
	namespace Utilities
	{
		public class ItemNotFound : IrisIMException
		{
			public ItemNotFound() : base()
			{}
		
			public ItemNotFound(string message) : base(message)
			{}
		}
	}
}