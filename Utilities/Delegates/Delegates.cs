
namespace IrisIM
{
	namespace Utilities
	{
	
		// Transceiver related delegates //
		public delegate void WriterOverload(Transceiver connection, string message);
		public delegate string ReaderOverload(Transceiver connection, string message);
		public delegate string WriteFilter(Message message);
		public delegate void MessageProcessor(Message message);
		public delegate void Disconnection(Transceiver connection);
		
		// IController MessagePump related delegates //
		public delegate void RemoveConnection(Transceiver connection);
	}
}