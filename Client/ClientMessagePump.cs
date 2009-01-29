using System;
using System.Threading;
using System.Collections;

using IrisIM.Utilities;

namespace IrisIM
{
	namespace Client
	{
		public class ClientMessagePump : MessagePump 
		{
			private event ClientEvent _client_event;
			private event UIEvent _ui_event;
			
			public event UIEvent ui_event
			{
				add
				{
					this._ui_event += value;
				}
				remove
				{
					this._ui_event -= value;
				}
			}
			
			public ClientMessagePump() : base()
			{}
			
			public ClientMessagePump(PluginManager plugins, ArrayList connections) : base(plugins, connections)
			{
				this.initialize();
				this.RegisterConnection(this.connection);
			}
			
			private void initialize()
			{
				foreach(ClientPlugin plugin in this.plugins.get_all())
				{
					this._client_event += new ClientEvent(plugin.process);
					Logger.log("Added plugin to client message event. ("+plugin.name+")", Logger.Verbosity.moderate);
				}
			}
			
			public void process_message(ClientMessage message)
			{
				try
				{
					this._client_event(message, this.connection);
				}
				catch(Exception e)
				{
					Logger.log("Error: No plugins loaded to process client message. (Dropping) "+e.Message+" \n"+e.StackTrace, Logger.Verbosity.moderate);
				}
			}
			
			public void process_message(UIMessage message)
			{
				try
				{
					this._ui_event(message);
				}
				catch
				{
					Logger.log("Error: No plugins loaded to process UI message. (Dropping)", Logger.Verbosity.moderate);
				}
			}
		}
	}
}