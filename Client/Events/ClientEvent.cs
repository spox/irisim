using IrisIM.Utilities;

namespace IrisIM
{
	namespace Client
	{
		public delegate void ClientEvent(ClientMessage message, Transceiver connection);
	}
}