
/* This is being used in place of an actual
   database at the moment. The base interface
   will stay the same which allows for the
   database to be integrated in the future and
   only this class will have to change */

using System;
using System.Data;
using System.Collections;
using System.Security.Cryptography;
using IrisIM.Utilities;
using Npgsql;

public class UserDB
{

	private string _user_table_def =
		"create table users(" +
		"username varchar(150) not null," +
		"alias varchar(50) not null," +
		"password char(64) not null," +
		"user_id bigserial primary key)";
	private string _db_connection_string;
	private IDbConnection _dbcon;
	private IDbCommand _cmd;
	private IDataReader _reader;
	private SHA512 _sha;
	private Configurator _config;
	
	public UserDB(Configurator conf)
	{
		this._config = conf;
		this._sha = SHA512.Create();
		try
		{	this.BuildDBConnectionString(conf);
			this._dbcon = new NpgsqlConnection(this._db_connection_string);
			this._cmd = this._dbcon.CreateCommand();
			this._dbcon.Open();
			this.CheckForTable();
		}
		catch(Exception e)
		{
			Logger.log("Failed to establish connection: "+e.Message+"\n"+e.StackTrace, Logger.Verbosity.moderate);
			throw new FatalException("UserManager failed to connect to the database. (Reason: "+e.Message+")");
		}
	}
	
	~UserDB()
	{
		this._dbcon.Close();
	}
	
	private void CheckForTable()
	{
		bool found = false;
		this._cmd.CommandText = "select table_name from information_schema.tables where table_schema = 'public'";
		this._reader = this._cmd.ExecuteReader();
		while(this._reader.Read())
		{
			string tablename = (string)this._reader["table_name"];
			found = (tablename == "users");
		}
		if(!found)
		{
			this._cmd.CommandText = this._user_table_def;
			this._cmd.ExecuteNonQuery();
		}
		this._reader.Close();
		this._reader = null;
	}
	
	private void BuildDBConnectionString(Configurator conf)
	{
		this._db_connection_string =
			"Server="+conf.get_value("dbServer")+";"+
			"Database="+conf.get_value("dbDB")+";"+
			"User ID="+conf.get_value("dbUser")+";"+
			"Password="+conf.get_value("dbPassword")+";";
	}
	
	public void AddUser(string username, string password, string alias)
	{
		if(UserRegistered(username))
		{
			throw new KeyExists("User is already registered.");
		}
		else
		{
			this._cmd.CommandText = 
				"insert into users (username, alias, password) values " +
				"('"+username+"', '"+alias+"', '"+this.HashPassword(password)+"')";
			this._cmd.ExecuteNonQuery();
		}
	}
	
	private bool UserRegistered(string username)
	{
		bool found = false;
		this._cmd.CommandText = "select user_id from users where username = '"+username+"'";
		this._reader = this._cmd.ExecuteReader();
		while(this._reader.Read())
		{
			found = true;
		}
		this._reader.Close();
		this._reader = null;
		return found;
	}
	
	private string HashPassword(string password)
	{
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(password);
		byte[] hash = this._sha.ComputeHash(bytes);
		string result = System.Text.Encoding.ASCII.GetString(hash);
		return result;
	}
	
	public void RemoveUser(string username)
	{
		if(this.UserRegistered(username))
		{
			this._cmd.CommandText = "delete from users where username = '"+username+"'";
			this._cmd.ExecuteNonQuery();
		}
		else
		{
			throw new ItemNotFound("User is not registered.");
		}
	}
	
	public UserInfo GetUser(string username, string password)
	{
		if(this.UserRegistered(username))
		{
			UserInfo info = null;
			string query = 
				"select alias, user_id from users where "+
				"username = '"+username+"' and "+
				"password = '"+this.HashPassword(password)+"'";
			this._cmd.CommandText = query;
			Logger.log("Query sent for login: "+query, Logger.Verbosity.moderate);
			this._reader = this._cmd.ExecuteReader();
			while(this._reader.Read())
			{
				info = new UserInfo();
				info.alias = (string)this._reader["alias"];
				info.id = (long)this._reader["user_id"];
				info.username = username;
			}
			if(info == null)
			{
				throw new ItemNotFound("Bad password.");
			}
			else
			{
				return info;
			}
		}
		else
		{
			if(this._config.exists("AutoRegister") && this._config.get_value("AutoRegister") == "yes")
			{
				this.AddUser(username, password, "");
				throw new Exception("You have been auto-registered. Please reconnect.");
			}
			else
			{
				throw new ItemNotFound("User is not registered");
			}
		}
	}
}

public class UserInfo
{
	public string username;
	public string password;
	public string alias;
	public long id;
	
	public UserInfo()
	{}
}