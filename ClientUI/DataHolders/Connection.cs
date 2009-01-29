// created on 5/12/2006 at 4:38 AM

namespace IrisIM
{
	namespace UI
	{
		public class Connection
		{
			private string _connectionName;
			private string _serverName;
			private int _port;
			
			public Connection()
			{
				this._connectionName = this._serverName = null;
				this._port = 0;
			}
			
			public Connection(string name, string server, int port)
			{
				this._connectionName = name;
				this._serverName = server;
				this.port = port;
			}
			
			public int port
			{
				set
				{
					if(value > 65335 || value < 1)
					{
						throw new OutOfRangeException("Invalid port number specified.");
					}
					else
					{
						this._port = value;
					}
				}
				get{ return this._port; }
			}
			
			public string connectionName
			{
				set{ this._connectionName = value; }
				get{ return this._connectionName; }
			}
			
			public string serverName
			{
				set{ this._serverName = value; }
				get{ return this._serverName; }
			}
		}
	}
}