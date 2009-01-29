using System;
using System.Xml;
using System.Collections;

namespace IrisIM
{
	namespace Utilities
	{
		public class Plugins : ComplexContent
		{
		
			private Hashtable _plugins;
		
			public Hashtable plugins
			{
				get{ return this._plugins; }
				set{ this._plugins = value; }
			}
		
			public Plugins() : base()
			{
				this._type = MessageContent.Type.Plugins;
				this._plugins = new Hashtable();
			}
		
			public Plugins(XmlElement content) : base()
			{
				this._type = MessageContent.Type.Plugins;
				this._plugins = new Hashtable();
				string requirement;
				string[] parts;
				XmlNodeList list = content.GetElementsByTagName("Plugin");
				PluginInfo info;
				foreach(XmlElement element in list)
				{
					info = new PluginInfo();
					if(element.HasAttribute("name"))
					{
						info.name = element.GetAttribute("name");
					}
					if(element.HasAttribute("version"))
					{
						info.version = element.GetAttribute("version");
					}
					if(element.HasAttribute("hash"))
					{
						info.hash = element.GetAttribute("hash");
					}
					if(element.HasAttribute("require"))
					{
						requirement = element.GetAttribute("require");
						parts = requirement.Split('-');
						info.required_name = parts[0];
						info.required_version = parts[1];
					}
					this._plugins[info.name] = info;
				} 
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement plugin = null;
				XmlAttribute attr = null;
				XmlElement plugins = document.CreateElement("Plugins");
				foreach(PluginInfo info in this._plugins.Values)
				{
					plugin = document.CreateElement("Plugin");
					attr = document.CreateAttribute("name");
					attr.Value = info.name;
					plugin.Attributes.Append(attr);
					attr = document.CreateAttribute("version");
					attr.Value = info.version;
					plugin.Attributes.Append(attr);
					attr = document.CreateAttribute("hash");
					attr.Value = info.hash;
					plugin.Attributes.Append(attr);
					attr = document.CreateAttribute("require");
					attr.Value = info.required_name + "-" + info.required_version;
					plugin.Attributes.Append(attr);
					plugins.AppendChild(plugin);
				}
				element.AppendChild(plugins);
			}
			
			public PluginInfo Get(string name)
			{
				if(this._plugins.ContainsKey(name))
				{
					return (PluginInfo)this._plugins[name];
				}
				else
				{
					throw new ItemNotFound();
				}
			}
		}
	}
}