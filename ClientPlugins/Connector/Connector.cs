
using System;
using System.Collections;

using IrisIM.Client;
using IrisIM.Utilities;

public class Connector : ClientPlugin
{

	private string _username;
	private string _password;
	private string _user_manager_hash;

	public Connector()
	{
		this._name = "Connector";
		this._version = "0.1";
	}
	
	override public void process(Message message)
	{
		if(message.type == Message.Type.Loop)
		{
			try
			{
				Event content = (Event)message.content;
				if(content.etype == "Encryption")
				{
					if(content.GetContent("status") == "crypted")
					{
						Logger.log("Sending authentication information.", Logger.Verbosity.moderate);
						this.SendAuthentication(this._username, this._password);
					}
				}
			}
			catch
			{
				Logger.log("Connector: Throwing out unwanted event message.", Logger.Verbosity.loud);
			}
		}
		try
		{
			Plugins content = (Plugins)message.content;
			PluginInfo userman = content.Get("UserManager");
			this._user_manager_hash = userman.hash;
			Logger.log("Connector: Found and stored server's User Manager hash code.", Logger.Verbosity.moderate);
		}
		catch
		{ /* do nothing */ }
		if(message.destination_plugin_hash == this._hash_code)
		{
			try
			{
				Response response = (Response)message.content;
				if(response.rtype == "success")
				{
					this.GoodResponse(message, response);
				}
				else
				{
					this.BadResponse(message, response);
				}
			}
			catch(Exception e)
			{
				Logger.log("Connector exception: "+e.Message, Logger.Verbosity.moderate);
			}
		}
	}
	
	override public void process(ClientMessage message, Transceiver connection)
	{
	Logger.log("Processing connection message.", Logger.Verbosity.moderate);
		if(message.type == "connect")
		{
			string server = message.Get("server");
			string port_s = message.Get("port");
			this._username = message.Get("username");
			this._password = message.Get("password");
			int port = Convert.ToInt32(port_s);
			Logger.log("Making connection.", Logger.Verbosity.moderate);
			if(this.EstablishConnection(server, port))
			{
				Message notice = new Message();
				notice.type = Message.Type.Loop;
				notice.creator_plugin_hash = this._hash_code;
				notice.origin = connection;
				Event connected = new Event();
				connected.etype = "connection";
				connected.AddContent("status", "connected");
				connected.type = BasicContent.Type.Event;
				notice.content = connected;
				this._controller.message_pump.process_message(notice);
				this.SendUINotice("Connected");
				Logger.log("Client now connected to server.", Logger.Verbosity.moderate);
				this.SendPluginListing(connection);
				Logger.log("Connection steps are now complete.", Logger.Verbosity.moderate);
			}
			else
			{
				this.SendUINotice("Failed");
				Logger.log("Failed to establish connection to server.", Logger.Verbosity.moderate);
			}
		}
	}
	
	private void GoodResponse(Message message, Response response)
	{
		this.SendUINotice(response.GetContent("command")+" successful");
	}
	
	private void BadResponse(Message message, Response response)
	{
		this._controller.connection.Close();
		this.Disconnect(message.origin);
		this.SendUIError(response.GetContent("reason"));
		this.SendUINotice("Disconnected.");
	}
	
	private void Disconnect(Transceiver connection)
	{
		Logger.log("Connector: Disconnected from server. Sending notice.", Logger.Verbosity.moderate);
		Message notice = new Message();
		notice.type = Message.Type.Loop;
		notice.creator_plugin_hash = this._hash_code;
		notice.origin = connection;
		Event connected = new Event();
		connected.etype = "connection";
		connected.AddContent("status", "disconnected");
		connected.type = BasicContent.Type.Event;
		notice.content = connected;
		this._controller.message_pump.process_message(notice);
	}
	
	private bool EstablishConnection(string servername, int port)
	{
		try
		{
			this._controller.connection.Connect(servername, port);
			this._controller.message_pump.Start();
			return true;
		}
		catch
		{
			return false;
		}
	}
	
	private bool SendAuthentication(string username, string password)
	{
		this.SendUINotice("Sending authentication information.");
		Message message = new Message();
		message.type = Message.Type.UserToServer;
		message.creator_plugin_hash = this._hash_code;
		message.destination_plugin_hash = this._user_manager_hash;
		Command contents = new Command();
		contents.ctype = "login";
		contents.type = MessageContent.Type.Command;
		contents.AddContent("username", username);
		contents.AddContent("password", password);
		message.content = contents;
		this._controller.connection.Write(message.DumpMessage());
		return true;
	}

	private void SendUIError(string message)
	{
		Logger.log("Sending UI Error message.", Logger.Verbosity.moderate);
		UIMessage uimessage = new UIMessage("error");
		uimessage.Add("message", message);
		this._controller.message_pump.process_message(uimessage);
	}

	private void SendUINotice(string notice)
	{
		UIMessage message = new UIMessage("connectionStatus");
		message.Add("status", notice);
		this._controller.message_pump.process_message(message);
	}
	
	private void SendPluginListing(Transceiver connection)
	{
		connection.Write(this.create_plugin_list(this._controller.plugins));
	}
	
	private Message create_plugin_list(PluginManager manager)
	{
		Message message = new Message();
		message.type = Message.Type.Plugins;
		message.creator_plugin_hash = this.GetHashCode().ToString();
		Plugins message_content = new Plugins();
		Hashtable plugins = new Hashtable();
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
	
	override public void initialize()
	{}
	
	override public void link_dependency(Object obj)
	{}
}