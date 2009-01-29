
using System;

namespace IrisIM
{
	namespace Utilities
	{
		public class NullException : IrisIMException
		{
			public NullException() : base()
			{}
			
			public NullException(string message) : base(message)
			{}
		}
	}
}