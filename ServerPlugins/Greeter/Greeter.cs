using System;
using System.Collections;
using System.Xml;
using System.IO;

using IrisIM.Server;
using IrisIM.Utilities;

public class Greeter : Plugin
{

	private string _name;
	private string _description;
	private string _version;
	private ArrayList _dependencies;
	private ArrayList _foreign_dependencies;
	private Controller _controller;
	
	private ArrayList _users;

	public string name
	{
		get{ return this._name;	}
		set{ this._name = value; }
	}
	
	public string version
	{
		get{ return this._version; }
	}
	
	public string description
	{
		get{ return this._description; }
		set{ this._description = value; }
	}
	
	public ArrayList dependencies
	{
		get{ return this._dependencies; }
	}
	
	public ArrayList foreign_dependencies
	{
		get{ return this._foreign_dependencies; }
	}
	
	public IController controller
	{
		set{ this._controller = (Controller)value; }
	}

	public Greeter()
	{
		this._users = new ArrayList();
		this._name = "Greeter";
		this._version = "0.1";
	}
	
	public void process(Message message)
	{
		if(!check_user(message.origin.GetHashCode()))
		{
			this.log_user(message.origin.GetHashCode());
			this.send_plugin_list(message.origin, this._controller.plugins);
			Logger.log("Plugin list has been sent to client.", Logger.Verbosity.moderate);
		}
	}
	
	public void initialize()
	{}
	
	public void link_dependency(Object obj)
	{}
	
	private bool check_user(int user_hash)
	{
		return this._users.Contains(user_hash);
	}
	
	private void log_user(int user_hash)
	{
		this._users.Add(user_hash);
	}
	
	private void send_plugin_list(Transceiver origin, PluginManager manager)
	{
		origin.Write(this.create_plugin_list(manager));
	}
	
	private Message create_plugin_list(PluginManager manager)
	{
		Message message = new Message();
		message.type = Message.Type.Plugins;
		message.creator_plugin_hash = this.GetHashCode().ToString();
		Plugins message_content = new Plugins();
		Hashtable plugins = new Hashtable();
		Plugins plugin_message = new Plugins();
		ArrayList loaded_plugins = manager.get_all();
		PluginInfo info;
		foreach(Plugin plug in loaded_plugins)
		{
			info = new PluginInfo();
			info.name = plug.name;
			info.hash = plug.GetHashCode().ToString();
			info.version = plug.version;
			plugins[info.name] = info;
		}
		message_content.plugins = plugins;
		message.content = message_content;
		return message;
	}
}