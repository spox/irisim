// created on 1/4/2006 at 7:27 PM

using System;
using System.Collections;

using IrisIM.Utilities;

namespace IrisIM
{
	namespace Server
	{
		public class Connections : Access
		{
			private Hashtable _connections;
			private int _index;
			
			public Connections()
			{
				this._connections = new Hashtable();
				this._index = 0;
			}
			
			public void add_connection(Transceiver connection)
			{
				this.request_access();
				int start_size;
				int end_size;
				start_size = this._connections.Count;
				if(!this._connections.Contains(connection))
				{
					this._connections[connection.GetHashCode()] = connection;
				}
				else
				{
					this.relieve_access();
					throw new Exception("Connection is already contained in list.");
				}
				end_size = this._connections.Count;
				this.relieve_access();
				if(end_size == start_size)
				{
					throw new Exception("Failed to add connection to list.");
				}
			}
			
			public void remove_connection(Transceiver connection)
			{
				this.request_access();
				if(this._connections.Contains(connection))
				{
					this._connections.Remove(connection);
				}
				else
				{
					this.relieve_access();
					throw new Exception("Connection is not contained in list.");
				}
				this.relieve_access();
			}
			
			public Transceiver get_at(string key)
			{
				Transceiver holder;
				this.request_access();
				if(this._connections.ContainsKey(key))
				{
					holder = (Transceiver)this._connections[key];
				}
				else
				{
					this.relieve_access();
					throw new Exception("Failed to find connection with given key.");
				}
				this.relieve_access();
				return holder;
			}
			
			public Transceiver get_next()
			{
				ArrayList connections;
				Transceiver holder;
				this.request_access();
				connections = new ArrayList(this._connections.Values);
				if(this._connections.Count == 0)
				{
					this.relieve_access();
					throw new Exception("No connections available.");
				}
				if(this._index >= connections.Count || this._index < 0)
				{
					this.reset_index();
				}
				holder = (Transceiver)connections[this._index++];
				this.relieve_access();
				return holder;
			}
			
			public void reset_index()
			{
				this._index = 0;
			}
		}
	}
}