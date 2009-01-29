
using System.Collections;

namespace IrisIM
{
	namespace Client
	{
		public class UIMessage : Informessage
		{
			public UIMessage(string type) : base(type)
			{}
			
			public UIMessage(string type, Hashtable info) : base(type, info)
			{}
		}
	}
}