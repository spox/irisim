
using System.Collections;
using IrisIM.Utilities;

public class UserCollection
{
	private Hashtable _users_by_name;
	private Hashtable _users_by_connection;
	
	public UserCollection()
	{
		this._users_by_connection = new Hashtable();
		this._users_by_name = new Hashtable();
	}
	
	public User Get(string username)
	{
		if(this._users_by_name.ContainsKey(username))
		{
			return (User)this._users_by_name[username];
		}
		else
		{
			throw new ItemNotFound("User is not logged in.");
		}
	}
	
	public User Get(Transceiver connection)
	{
		if(this._users_by_connection.ContainsKey(connection))
		{
			return (User)this._users_by_connection[connection];
		}
		else
		{
			throw new ItemNotFound("User is not logged in.");
		}
	}
	
	public void Add(User user)
	{
		if(this._users_by_connection.ContainsKey(user.connection))
		{
			throw new KeyExists();
		}
		else
		{
			this._users_by_connection[user.connection] = user;
			this._users_by_name[user.username] = user;
		}
	}
	
	public void Remove(string username)
	{
		if(this._users_by_name.ContainsKey(username))
		{
			this.RemoveUser((User)this._users_by_name[username]);
		}
		else
		{
			throw new ItemNotFound();
		}
	}
	
	public void Remove(Transceiver connection)
	{
		if(this._users_by_connection.ContainsKey(connection))
		{
			this.RemoveUser((User)this._users_by_connection[connection]);
		}
		else
		{
			throw new ItemNotFound();
		}
	}
	
	public bool Exists(string username)
	{
		return this._users_by_name.ContainsKey(username);
	}
	
	public bool Exists(Transceiver connection)
	{
		return this._users_by_connection.ContainsKey(connection);
	}
	
	private void RemoveUser(User info)
	{
		this._users_by_name.Remove(info.username);
		this._users_by_connection.Remove(info.connection);
	}
}