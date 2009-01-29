// created on 1/5/2006 at 10:11 AM

using System;
using System.IO;
using System.Reflection;
using System.Collections;

using IrisIM.Utilities;

namespace IrisIM
{
	namespace Utilities
	{
		public class PluginManager : Access
		{
			private Hashtable _plugins;
			private string _plugin_path;
			private IController _controller;
			
			public PluginManager(IController controller)
			{
				this._controller = controller;
				this._plugins = new Hashtable();
				this._plugin_path = Path.GetFullPath("plugins")  + Path.DirectorySeparatorChar.ToString();
				Logger.log("Plugin path set to: " + this._plugin_path, Logger.Verbosity.moderate);
			}
			
			private Plugin dependency_check(Plugin plugin)
			{
				string plug;
				ArrayList plugins = plugin.dependencies;
				foreach(Object obj in plugins)
				{
					plug = (string)obj;
					if(this._plugins.ContainsKey(plug))
					{
						plugin.link_dependency(this._plugins[plug]);
					}
					else
					{
						throw new Exception("Dependency check failed. Missing plugin ("+plug+")");
					}
				}
				return plugin;
			}
			
			public void register(string plugin_name)
			{
				try
				{
					Plugin plugin;
					Assembly ass = Assembly.LoadFile(this._plugin_path + plugin_name + ".dll");
					if(ass == null)
					{
						Logger.log("Failed to load plugin ("+plugin_name+"). Perhaps file is missing.", Logger.Verbosity.moderate);
						throw new Exception("Failed to load plugin.");
					}
					foreach(Type type in ass.GetTypes())
					{
						if(type.IsClass)
						{
							Logger.log("Discovered new class. ("+type.FullName+")", Logger.Verbosity.moderate);
							if(type.GetInterface("Plugin") == null)
							{
								Logger.log("Discovered class is not a valid plugin. ("+type.FullName+")", Logger.Verbosity.moderate);
							}
							else
							{
								try
								{
									plugin = (Plugin)Activator.CreateInstance(type);
									plugin.controller = this._controller;
									plugin.initialize();
									this._plugins[type.FullName] = plugin;
									Logger.log("New plugin has been added to the system. ("+type.FullName+")", Logger.Verbosity.moderate);
								}
								catch(Exception e)
								{
									throw new Exception("Failed to create new instance of discovered plugin. ("+type.FullName+") "+e.Message + " "+ e.StackTrace);
								}
							}
						}
					}
				}
				catch(Exception e)
				{
					throw e;
				}
			}
			
			public void unregister(Plugin plugin)
			{
				this.request_access();
				if(this._plugins.Contains(plugin))
				{
					this._plugins.Remove(plugin);
					Logger.log("Plugin "+plugin.name+" has been unregistered.", Logger.Verbosity.moderate);
				}
				else
				{
					throw new Exception("Plugin "+plugin.name+" is not a registered plugin.");
				}
				this.relieve_access();
			}
			
			public ArrayList get_all()
			{
				if(this._plugins.Count == 0)
				{
					throw new Exception("No plugins registered.");
				}
				else
				{
					return new ArrayList(this._plugins.Values);
				}
			}
		}
	}
}