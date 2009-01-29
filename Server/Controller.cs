using System;
using System.Collections;
using IrisIM.Server;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace Server
	{
		public class Controller : IController
		{		
			private PluginManager _plugins;
			private MessagePump _message_pump;
			private ArrayList _connections;
			private Listener _base_listener;
			private int _base_port;
			private Configurator _configuration;
			
			public int base_port
			{
				get
				{
					return this._base_port;
				}
				set
				{
					if(value > 65335)
					{
						Logger.log("Port number exceeded maximum port number. Setting to default port number.", Logger.Verbosity.quiet);
						this._base_port = -1;
						throw new IrisIMException("Port value exceeds maximum valid port number.");
					}
					this._base_port = value;
				}
			}
			
			public PluginManager plugins
			{
				get{ return this._plugins; }
			}
			
			public MessagePump message_pump
			{
				get{ return this._message_pump; }
			}
			
			public ArrayList connections
			{
				get{ return this._connections; }
			}
			
			public Listener base_listener
			{
				get{ return this._base_listener; }
			}

			public Configurator configuration
			{
				get{ return this._configuration; }
			}

			public Controller()
			{
				this._configuration = new Configurator();
				this.initialize();
			}
			
			private void initialize()
			{
				string plugin_base;
				string[] plugins;
				try{ this._base_port = this._configuration.get_asInt("ListenPort"); }
				catch(Exception e){ this._base_port = -1; }
				this._connections = new ArrayList();
				this._plugins = new PluginManager(this);
				
				try
				{
					plugin_base = this._configuration.get_value("Plugins");
					plugins = plugin_base.Split('|');
					foreach(string plugin in plugins)
					{
						try
						{
							this._plugins.register(plugin);
						}
						catch(Exception e)
						{
							Logger.log("Error detected while loading plugin ("+plugin+")", Logger.Verbosity.moderate);
							Logger.log("'- "+e.Message, Logger.Verbosity.moderate);
						}
					}
				}
				catch(ItemNotFound e)
				{
					Logger.log("No plugins specified to load in configuration file.", Logger.Verbosity.moderate);
				}
				catch(Exception e)
				{
					Logger.log("Failed to load plugins. No plugins loaded.", Logger.Verbosity.moderate);
				}
				this.LinkPlugins();
				this._base_listener = this._base_port == -1 ? new Listener(this._connections) : new Listener(this._base_port, this._connections);				
				this._message_pump = new MessagePump(this._plugins, this._connections);
				this._message_pump.deleteConnection = this.DeleteConnection;
				this._base_listener.connection_announcement += new NewConnection(this._message_pump.RegisterConnection);
			}
			
			private void LinkPlugins()
			{
				ArrayList list = this._plugins.get_all();
				foreach(Plugin plug in list)
				{
					foreach(Plugin p in list)
					{
						try{plug.link_dependency(p);}
						catch{}
					}
				}
			}
			
			public void online()
			{
				this._base_listener.start();
				this._message_pump.Start();
			}
			
			public void offline()
			{
				this._base_listener.stop();
			}
			
			public void suspend()
			{
				this._message_pump.Pause();
			}
			
			public void resume()
			{
				this._message_pump.Resume();
			}
			
			public void shutdown()
			{
				this.offline();
				Logger.log("Shutting down the server.", Logger.Verbosity.quiet);
				this.DisconnectTransceivers();
				this._connections = null;
				this._base_listener.stop();
				this._base_listener = null;
				Logger.log("Shutdown complete.", Logger.Verbosity.quiet);
				return;
			}
			
			private void DisconnectTransceivers()
			{
				
				foreach(Transceiver c in this._connections)
				{
					c.Close();
					this._connections.Remove(c);
				}
			}
			
			public void DeleteConnection(Transceiver connection)
			{
				this._connections.Remove(connection);
			}
		}
	}
}