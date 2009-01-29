using System;

namespace IrisIM
{
	namespace UI
	{
		class UIException : Exception
		{
			public UIException(string message) : base(message)
			{}
		}
	}
}