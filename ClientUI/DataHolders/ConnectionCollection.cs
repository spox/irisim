using System;
using System.Collections;

namespace IrisIM
{
	namespace UI
	{
		class ConnectionCollection
		{
			Hashtable collection;
			
			public ConnectionCollection()
			{
				this.collection = new Hashtable();
			}
			
			public void Add(Connection con)
			{
				if(this.collection.ContainsKey(con.connectionName))
				{
					throw new KeyExistsException(con.connectionName + " already exists in collection.");
				}
				else
				{
					this.collection[con.connectionName] = con;
				}
			}
			
			public void ForceAdd(Connection con)
			{
				this.collection[con.connectionName] = con;
			}
			
			public void Modify(Connection con)
			{
				this.ForceAdd(con);
			}
			
			public void Remove(Connection con)
			{
				this.collection.Remove(con.connectionName);
			}
			
			public Connection Get(string name)
			{
				if(this.collection.ContainsKey(name))
				{
					return (Connection)this.collection[name];
				}
				else
				{
					throw new NotFoundException("No key found matching: "+name.ToString());
				}
			}
			
			public bool Exists(string name)
			{
				return this.collection.ContainsKey(name);
			}
			
			public ICollection Names()
			{
				return this.collection.Keys;
			}
		}
	}
}