namespace IrisIM
{
	namespace Utilities
	{
		public interface IController
		{
			void online();
			void offline();
			void suspend();
			void resume();
			void shutdown();
			void DeleteConnection(Transceiver connection);
		}
	}
}