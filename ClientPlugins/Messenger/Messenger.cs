

using System;
using System.Collections;

using IrisIM.Client;
using IrisIM.Utilities;

public class Messenger : ClientPlugin
{

	private Hashtable _user_cache;
	private string _user_manager_hash;

	public Messenger()
	{
		this._name = "Messenger";
		this._version = "0.1";
		this._user_cache = new Hashtable();
	}
	
	override public void process(Message message)
	{
		try
		{
			if(message.destination_plugin_hash == this._hash_code)
			{
				if(message.content.type == MessageContent.Type.Response)
				{
					Response res = (Response)message.content;
					if(res.rtype == "failure")
					{
						this.SendUIError(res.GetContent("reason"));
					}
					else
					{
						this.StartChat(res.GetContent("username"), res.GetContent("alias"));
					}
				}
			}
			else if(message.content.type == MessageContent.Type.Information)
			{
				Information info = (Information)message.content;
				if(info.Exists("username") && info.Exists("alias"))
				{
					this.StartChat(info.GetContent("username"), info.GetContent("alias"));
				}
			}
			else if(message.type == Message.Type.UserToUser)
			{
				this.ProcessChatMessage(message);
			}
		}
		catch(Exception e)
		{
			Logger.log("Messenger: Exception caught -> "+e.Message, Logger.Verbosity.moderate);
		}
		try
		{
			Plugins content = (Plugins)message.content;
			PluginInfo userman = content.Get("UserManager");
			this._user_manager_hash = userman.hash;
			Logger.log("Messenger: Found and stored server's User Manager hash code.", Logger.Verbosity.moderate);
		}
		catch
		{ /* do nothing */ }
	}
	
	override public void process(ClientMessage message, Transceiver connection)
	{
		if(message.type == "userRequest")
		{
			if(this._user_cache.ContainsKey(message.Get("username")))
			{
				this.StartChat(message.Get("username"), (string)this._user_cache[message.Get("username")]);
			}
			else
			{
				this.SendRequest(message.Get("username"), connection);
			}
		}
		else if(message.type == "userMessage")
		{
			this.SendUserMessage(message, connection);
		}
	}
	
	private void SendUserMessage(ClientMessage cmessage, Transceiver connection)
	{
		Message message = new Message();
		message.type = Message.Type.UserToUser;
		message.destination_plugin_hash = cmessage.Get("destination");
		Chat content = new Chat();
		content.AddContent("message", cmessage.Get("message"));
		content.AddContent("time", cmessage.Get("time"));
		message.content = content;
		connection.Write(message);
		Logger.log("Messenger: Chat message sent.", Logger.Verbosity.moderate);
	}
	
	private void SendRequest(string username, Transceiver connection)
	{
		Message message = new Message();
		message.type = Message.Type.ServerToUserPlugin;
		message.creator_plugin_hash = this._hash_code;
		message.destination_plugin_hash = this._user_manager_hash;
		Command content = new Command();
		content.ctype = "information";
		content.AddContent("username", username);
		message.content = content;
		connection.Write(message);
		Logger.log("Messenger: Requesting information for chat.", Logger.Verbosity.moderate);
		this.SendUINotice("Requesting information.");
	}
	
	private void StartChat(string username, string alias)
	{
		Logger.log("Messenger: Sending notice of new chat.", Logger.Verbosity.moderate);
		ClientMessage mess = new ClientMessage("newUser");
		mess.Add("username", username);
		this._controller.message_pump.process_message(mess);
		this.SendUINotice("New chat has be started.");
		UIMessage message = new UIMessage("startChat");
		message.Add("username", username);
		message.Add("alias", alias);
		this._controller.message_pump.process_message(message);
	}

	private void ProcessChatMessage(Message message)
	{
		UIMessage uimess = new UIMessage("chatMessage");
		Chat contents = (Chat)message.content;
		uimess.Add("originator", message.creator_plugin_hash);
		uimess.Add("message", contents.GetContent("message"));
		this._controller.message_pump.process_message(uimess);
		Logger.log("Messenger: Processed incoming chat message.", Logger.Verbosity.moderate);
	}
	
	override public void initialize()
	{}
	
	override public void link_dependency(Object o)
	{}

	private void SendUINotice(string notice)
	{
		UIMessage message = new UIMessage("connectionStatus");
		message.Add("status", notice);
		this._controller.message_pump.process_message(message);
	}
	
	private void SendUIError(string message)
	{
		Logger.log("Sending UI Error message.", Logger.Verbosity.moderate);
		UIMessage uimessage = new UIMessage("error");
		uimessage.Add("message", message);
		this._controller.message_pump.process_message(uimessage);
	}
	
}