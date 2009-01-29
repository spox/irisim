
using System;
using System.Collections;
using IrisIM.Utilities;

namespace IrisIM
{
	namespace Client
	{
		abstract public class ClientPlugin : Plugin
		{
			protected string _name;
			protected string _description;
			protected string _version;
			protected string _hash_code;
			protected ArrayList _dependencies;
			protected ArrayList _foreign_dependencies;
			protected Controller _controller;

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
				get{ return this._foreign_dependencies; }
			}
			
			public IController controller
			{
				set{ this._controller = (Controller)value; }
			}

			public ClientPlugin() : base()
			{
				this._hash_code = this.GetHashCode().ToString();
			}
			
			abstract public void initialize();
			abstract public void link_dependency(Object o);
			
			abstract public void process(Message message);
			abstract public void process(ClientMessage message, Transceiver connection);
		}
	}
}