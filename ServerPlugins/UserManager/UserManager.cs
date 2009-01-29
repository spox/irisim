using IrisIM.Server;
using IrisIM.Utilities;

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

public class UserManager : Plugin
{
	private string _name;
	private string _description;
	private string _version;
	private ArrayList _dependencies;
	private ArrayList _foreign_dependencies;
	private Controller _controller;

	public string name
	{
		get{ return this._name; }
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
		get{ return this._foreign_dependencies;	}
	}
	
	public IController controller
	{
		set{ this._controller = (Controller)value; }
	}

	private UserCollection _users;
	private UserDB _user_database;
	private string _hash_code;

	public UserManager()
	{
		this._name = "UserManager";
		this._version = "0.1";
		this._users = new UserCollection();
	}
	
	/*
	private void MakeTempUsers()
	{
		UserInfo info = new UserInfo();
		info.alias = "spox";
		info.username = "spox";
		info.password = "password";
		this._user_database.AddUser(info);
		info = new UserInfo();
		info.alias = "james";
		info.username = "james";
		info.password = "password";
		this._user_database.AddUser(info);
		info = new UserInfo();
		info.alias = "cj";
		info.username = "cj";
		info.password = "password";
		this._user_database.AddUser(info);
		
	}
	*/
	public void initialize()
	{
		this._user_database = new UserDB(this._controller.configuration);
		this._hash_code = this.GetHashCode().ToString();
	}
	
	public void process(Message message)
	{
		if(message.destination_plugin_hash == this._hash_code || message.type == Message.Type.UserToUser)
		{
			this.RunMessage(message);
		}
	}
	
	public void link_dependency(Object obj)
	{}
	
	private void RunMessage(Message message)
	{
		switch(message.content.type)
		{
			case MessageContent.Type.Command:
				try
				{
					Command command = (Command)message.content;
					this.ProcessCommand(message, command);
				}
				catch
				{
					Logger.log("UserManager: Failed to cast to Command type. Mislabeled type?", Logger.Verbosity.moderate);
				}
			break;
			case MessageContent.Type.Response:
				try
				{
					Response response = (Response)message.content;
					this.ProcessResponse(message, response);
				}
				catch
				{
					Logger.log("UserManager: Failed to cast to Response type. Mislabeled type?", Logger.Verbosity.moderate);
				}
			break;
			case MessageContent.Type.Chat:
				try
				{
					//this.ProcessChat(message);
				}
				catch(Exception e)
				{
					Logger.log("UserManager: Failed processing chat message. ("+e.Message+")", Logger.Verbosity.moderate);
				}
			break;
			default:
				Logger.log("UserManager: Unknown message type. Not setup to handle this message.", Logger.Verbosity.moderate);
			break;
		}
	}
	
	private void ProcessCommand(Message message, Command command)
	{
		try
		{
			switch(command.ctype)
			{
				case "login":
					this.Login(command.GetContent("username"), command.GetContent("password"), message);
					break;
				case "logout":
					this.Logout(command.GetContent("username"), message);
					break;
				case "register":
					this.Register(command.GetContent("username"), command.GetContent("password"), command.GetContent("alias"));
					break;
				case "status":
					this.Status(command.GetContent("username"), message);
					throw new Exception("Punching out so we don't send an error.");
					break;
				case "information":
					this.UserInformation(command.GetContent("username"), message);
					break;
				default:
					throw new UnknownType("Command not valid.");
					break;
			}
		}
		catch(IrisIMException e)
		{
			Message resp = new Message();
			resp.creator_plugin_hash = this._hash_code;
			resp.destination_plugin_hash = message.creator_plugin_hash;
			resp.type = Message.Type.ServerToUserPlugin;
			Response content = new Response();
			content.rtype = "failure";
			content.AddContent("command", command.ctype);
			content.AddContent("reason", e.Message);
			resp.content = content;
			message.origin.Write(resp);
		}
		catch(Exception e)
		{
			Message resp = new Message();
			resp.creator_plugin_hash = this._hash_code;
			resp.destination_plugin_hash = message.creator_plugin_hash;
			resp.type = Message.Type.ServerToUserPlugin;
			Response content = new Response();
			content.rtype = "failure";
			content.AddContent("command", command.ctype);
			content.AddContent("reason", e.Message);
			resp.content = content;
			message.origin.Write(resp);
			Logger.log("UserManager ProcessCommand error:", Logger.Verbosity.moderate);
			Logger.log(e.Message+"\n"+e.StackTrace, Logger.Verbosity.moderate);
		}
	}
	
	private void ProcessResponse(Message message, Response response)
	{
	}
/*
	private void ProcessChat(Message message)
	{
		User info = this.GetUser(message.origin);
		message.creator_plugin_hash = info.username;
		User dest = this.GetUser(message.creator_plugin_hash);
		dest.connection.Write(message);
	}
*/	
	private void Register(string username, string password, string alias)
	{
		this._user_database.AddUser(username, password, alias);
	}
	
	private void Login(string username, string password, Message message)
	{
		Logger.log("attempting login.", Logger.Verbosity.moderate);
		try
		{
			UserInfo info = this._user_database.GetUser(username, password);
			User user = new User(info.username, info.alias, message.origin);
			this._users.Add(user);
			Message response = new Message();
			response.type = Message.Type.ServerToUserPlugin;
			response.creator_plugin_hash = this._hash_code;
			response.destination_plugin_hash = message.creator_plugin_hash;
			Response content = new Response();
			content.rtype = "success";
			content.AddContent("command", "login");
			response.content = content;
			message.origin.Write(response);
		}
		catch(IrisIMException e)
		{
			throw new ItemNotFound("Invalid username/password");
		}
	}
	
	private void Logout(string username, Message message)
	{
		this._users.Remove(message.origin);
	}
	
	private void Status(string username, Message message)
	{
		User info = this._users.Get(username);
		string stats = info.GetStatus();
		Message statusMessage = new Message();
		statusMessage.type = Message.Type.ServerToUserPlugin;
		statusMessage.creator_plugin_hash = this._hash_code;
		statusMessage.destination_plugin_hash = message.creator_plugin_hash;
		Command command = (Command)message.content;
		Response contents = new Response();
		contents.AddContent(command.ctype, stats);
		statusMessage.content = contents;
		message.origin.Write(statusMessage);
	}
	
	private void UserInformation(string username, Message message)
	{
		User userinfo = this.GetUser(username);
		Message response = new Message();
		response.type = Message.Type.ServerToUserPlugin;
		response.creator_plugin_hash = this._hash_code;
		response.destination_plugin_hash = message.creator_plugin_hash;
		Response content = new Response();
		content.rtype = "success";
		content.AddContent("command", "information");
		content.AddContent("username", userinfo.username);
		content.AddContent("alias", userinfo.alias);
		response.content = content;
		message.origin.Write(response);
		User requesterInfo = this.GetUser(message.origin);
		Message information = new Message();
		information.type = Message.Type.ServerToUser;
		information.creator_plugin_hash = this._hash_code;
		Information con = new Information();
		con.AddContent("username", requesterInfo.username);
		con.AddContent("alias", requesterInfo.alias);
		information.content = con;
		userinfo.connection.Write(information);
	}
	
	public User GetUser(string username)
	{
		return this._users.Get(username);
	}
	
	public User GetUser(Transceiver connection)
	{
		return this._users.Get(connection);
	}
}
