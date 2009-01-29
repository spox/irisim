
using System.Collections;

namespace IrisIM
{
	namespace Client
	{
		public class ClientMessage : Informessage
		{
			public ClientMessage(string type) : base(type)
			{}
			
			public ClientMessage(string type, Hashtable info) : base(type, info)
			{}
		}
	}
}