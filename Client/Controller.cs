using System;
using System.Collections;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace Client
	{
		public class Controller : IController
		{		
			private PluginManager _plugins;
			private ClientMessagePump _message_pump;
			private Configurator _configuration;
			private ArrayList _connections;
			private Transceiver _connection;
			
			public PluginManager plugins
			{
				get{ return this._plugins; }
			}
			
			public ClientMessagePump message_pump
			{
				get{ return this._message_pump; }
			}
			
			public Transceiver connection
			{
				get{ return this._connection; }
			}
			
			public ArrayList connections
			{
				get{ return this._connections; }
			}

			public Configurator configuration
			{
				get{ return this._configuration; }
			}

			public Controller()
			{
				this._configuration = new Configurator();
				this._connections = new ArrayList();
				this.initialize();
			}
			
			private void initialize()
			{
				string plugin_base;
				string[] plugins;
				this._connection = new Transceiver();
				this._connections.Add(this._connection);
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
								
				this._message_pump = new ClientMessagePump(this._plugins, this._connections);
			}
			
			public void online()
			{
				this._message_pump.Start();
			}
			
			public void offline()
			{
				Transceiver connection = (Transceiver)this._connections[0];
				connection.Close();
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
				Logger.log("Shutdown complete.", Logger.Verbosity.quiet);
				return;
			}
			
			public void DeleteConnection(Transceiver connection)
			{ /* We don't delete connections in the client yet */ }
		}
	}
}