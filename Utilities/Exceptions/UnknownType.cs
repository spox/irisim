using System;

namespace IrisIM
{
	namespace Utilities
	{
		public class UnknownType : IrisIMException
		{
			public UnknownType() : base()
			{}
			
			public UnknownType(string message) : base(message)
			{}
		}
	}
}