using System;
using System.Collections;
using System.Threading;
using System.Text;

namespace IrisIM
{
	namespace Utilities
	{
		public class MessagePump
		{
			private PluginManager _plugins;
			private bool _run;
			private object _run_lock;
			private ArrayList _connections;
			private event MessageProcessor _message_processor;
			private RemoveConnection _deleteConnection;
			
			public ArrayList connections
			{
				get{ return this._connections; }
			}
			
			public Transceiver connection
			{
				get
				{
					if(this._connections.Count < 1)
					{
						throw new NullException("No transceiver objects are registered.");
					}
					return (Transceiver)this._connections[0];
				}
			}
			
			public RemoveConnection deleteConnection
			{
				get{ return this._deleteConnection; }
				set{ this._deleteConnection = value; }
			}
			
			public PluginManager plugins
			{
				get{ return this._plugins; }
			}
			
			public MessagePump()
			{
				this._run = false;
				this._run_lock = new object();
			}
			
			public MessagePump(PluginManager plugins, ArrayList connections)
			{
				this._run = false;
				this._plugins = plugins;
				this.set_processors();
				this._connections = connections;
				this._run_lock = new object();
			}
			
			private void set_processors()
			{
				Plugin plugin;
				try
				{
					ArrayList plugins = this._plugins.get_all();
					foreach(object obj in plugins)
					{
						plugin = (Plugin)obj;
						this._message_processor += new MessageProcessor(plugin.process);
						Logger.log("Added plugin to event. ("+plugin.name+")", Logger.Verbosity.moderate);
					}
				}
				catch(Exception e)
				{
					Logger.log("Exception caught while setting processors in MessagePump: "+e.Message, Logger.Verbosity.moderate);
				}
			}

			public void Start()
			{
				this.Resume();
			}

			public void Pause()
			{
				lock(this._run_lock)
				{
					this._run = false;
				}
			}
			
			public void Resume()
			{
				lock(this._run_lock)
				{
					this._run = true;
				}
			}
			
			public void process_message(Message message)
			{
				while(!this._run)
				{
					Thread.Sleep(10);
				}
				lock(this._run_lock)
				{
					this._message_processor(message);
				}
			}
			
			public void RegisterConnection(Transceiver connection)
			{
				connection.message_processor += new MessageProcessor(this.process_message);
				connection.disconnection += new Disconnection(this.Disconnect);
			}
			
			public void Disconnect(Transceiver connection)
			{
				connection.Close();
				this._deleteConnection(connection);
				this.NotifyOfDisconnect(connection);
				Logger.log("Connection has been closed and removed. Code: "+connection.GetHashCode(), Logger.Verbosity.moderate);
			}
			
			private void NotifyOfDisconnect(Transceiver connection)
			{
				Message message = new Message();
				message.type = Message.Type.Loop;
				message.origin = connection;
				Event contents = new Event();
				contents.etype = "Disconnect";
				message.content = contents;
				this.process_message(message);
			}
		}
	}
}