using System.Collections;

using IrisIM.Utilities;

namespace IrisIM
{
	namespace Client
	{
		public class Informessage
		{
			private string _type;
			private Hashtable _information;
			
			public string type
			{
				get{ return this._type; }
				set{ this._type = value; }
			}
			
			public Hashtable information
			{
				get{ return this._information; }
				set{ this._information = value; }
			}
			
			public Informessage()
			{
				this._type = "unknown";
				this._information = new Hashtable();
			}
			
			public Informessage(string type)
			{
				this._type = type;
				this._information = new Hashtable();
			}
			
			public Informessage(string type, Hashtable information)
			{
				this._type = type;
				this._information = information;
			}
			
			public void Add(string key, string val)
			{
				if(this._information.ContainsKey(key))
				{
					throw new KeyExists();
				}
				else
				{
					this._information[key] = val;
				}
			}
			
			public void Remove(string key)
			{
				if(this._information.ContainsKey(key))
				{
					this._information.Remove(key);
				}
				else
				{
					throw new ItemNotFound();
				}
			}
			
			public void Modify(string key, string val)
			{
				if(this._information.ContainsKey(key))
				{
					this._information[key] = val;
				}
				else
				{
					throw new ItemNotFound();
				}
			}
			
			public string Get(string key)
			{
				if(this._information.ContainsKey(key))
				{
					return (string)this._information[key];
				}
				else
				{
					throw new ItemNotFound();
				}
			}
			
			public bool Exists(string key)
			{
				return this._information.ContainsKey(key);
			}
		}
	}
}