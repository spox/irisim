
using System;
using System.Xml;
using System.Collections;

using IrisIM.Server;
using IrisIM.Utilities;

public class UserChat : Plugin
{

	private string _name;
	private string _description;
	private string _version;
	private ArrayList _dependencies;
	private ArrayList _foreign_dependencies;
	private Controller _controller;

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
	
	private string _hash_code;
	private UserManager _user_manager;

	public UserChat()
	{
		this._dependencies = new ArrayList();
		this._name = "UserChat";
		this._version = "0.1";
		this._user_manager = null;
		this._dependencies.Add("UserManager");
	}
	
	public void process(Message message)
	{
		if(this._user_manager == null)
		{
			Logger.log("UserManager is not linked. This plugin will not process messages.", Logger.Verbosity.moderate);
			return;
		}
		if(message.destination_plugin_hash == this._hash_code)
		{
			try
			{
				if(message.content.type == MessageContent.Type.Chat)
				{
					this.ProcessChatMessage(message);
				}
			}
			catch(Exception e)
			{
				Logger.log("Received directed message but is incorrect type. Dropped.", Logger.Verbosity.moderate);
			}
		}
		else
		{
			if(message.type == Message.Type.UserToUser)
			{
				try
				{
					this.ProcessChatMessage(message);
				}
				catch(Exception e)
				{
					Logger.log("UserChat: Failed to process message: "+e.Message, Logger.Verbosity.moderate);
				}
			}
		}
	}
		
	public void initialize()
	{
		this._hash_code = this.GetHashCode().ToString();
	}
	
	public void link_dependency(Object obj)
	{
		try
		{
			this._user_manager = (UserManager)obj;
			Logger.log("UserChats: Successfully linked to UserManager.", Logger.Verbosity.moderate);
		}
		catch{}
	}
	
	public void ProcessChatMessage(Message message)
	{
		User destination = this._user_manager.GetUser(message.destination_plugin_hash);
		User sender = this._user_manager.GetUser(message.origin);
		message.creator_plugin_hash = sender.username;
		string send = this.BuildMessage(message);
		Logger.log("Sending message to: "+message.destination_plugin_hash, Logger.Verbosity.moderate);
		Logger.log("Got connection to send to: "+destination.username, Logger.Verbosity.moderate);
		destination.connection.Write(send);
	}
	
	private string BuildMessage(Message message)
	{
		Unknown content = (Unknown)message.content;
		string full = content.message_data;
		XmlDocument document = new XmlDocument();
		document.LoadXml(full);
		XmlNodeList list = document.GetElementsByTagName("Message");
		XmlAttribute attr = document.CreateAttribute("creator");
		attr.Value = message.creator_plugin_hash;
		XmlElement element = (XmlElement)list.Item(0);
		element.Attributes.Append(attr);
		return document.InnerXml;
	}
}