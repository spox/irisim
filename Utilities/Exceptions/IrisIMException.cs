using System;

namespace IrisIM
{
	namespace Utilities
	{
		public class IrisIMException : Exception
		{
			public IrisIMException() : base()
			{}
			
			public IrisIMException(string message) : base(message)
			{}
		}
	}
}