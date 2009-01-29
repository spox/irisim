using System;
using IrisIM;
using IrisIM.Utilities;

public class User
{

	enum Status{ online = 0, idle, away, distant }

	private string _username;
	private string _alias;
	private Transceiver _connection;
	private Status _user_status;
	private string _away_message;
	private DateTime _last_action;
	
	public string username
	{
		get{ return this._username; }
	}
	
	public string alias
	{
		get{ return this._alias; }
	}
	
	public string away_message
	{
		set{ this._away_message = value; }
		get{ return this._away_message; }
	}
	
	public Transceiver connection
	{
		get{ return this._connection; }
	}
	
	public User(string username, string alias, Transceiver connection)
	{
		this._connection = connection;
		this._username = username;
		this._alias = alias;
		this._user_status = Status.online;
		this._last_action = new DateTime();
	}
	
	/**
	 * User status on the network
	**/
	
	public bool IsOnline()
	{
		return this._user_status == Status.online;
	}
	
	public bool IsIdle()
	{
		return this._user_status == Status.idle;
	}
	
	public bool IsAway()
	{
		return this._user_status == Status.away;
	}
	
	public bool IsDistant()
	{
		return this._user_status == Status.distant;
	}
	
	public string GetStatus()
	{
		string status = null;
		switch(this._user_status)
		{
			case Status.away:
				status = "away";
				break;
			case Status.distant:
				status = "distant";
				break;
			case Status.idle:
				status = "idle";
				break;
			case Status.online:
				status = "online";
				break;
			default:
				status = "unknown";
				break;
		}
		return status;
	}	
}