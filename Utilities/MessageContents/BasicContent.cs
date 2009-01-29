
using System;
using System.Xml;
using System.Collections;

namespace IrisIM
{
	namespace Utilities
	{
		public class BasicContent : MessageContent
		{
			
			protected Hashtable _content;
			
			public Hashtable content
			{
				get{ return this._content; }
				set{ this._content = value; }
			}
			
			public BasicContent() : base()
			{
				this._raw_content = null;
				this._content = new Hashtable();
				this._type = MessageContent.Type.Unknown;
			}
			
			public BasicContent(XmlElement message) : base(message)
			{
				this._raw_content = message;
				this._content = new Hashtable();
				this._type = MessageContent.Type.Unknown;
				this.DisassembleContent();
			}
			
			public BasicContent(BasicContent clone) : base()
			{
				this._content = clone._content;
				this._raw_content = clone._raw_content;
				this._type = clone._type;
			}
			
			private void DisassembleContent()
			{
				this.HashContent(this.DiscoverType());
			}
			
			private void HashContent(XmlElement element)
			{
				if(element == null)
				{
					return;
				}
				string name = "";
				string val = "";
				XmlNodeList list = null;
				list = element.GetElementsByTagName("Value");
				foreach(XmlElement elm in list)
				{
					name = elm.GetAttribute("name");
					val = elm.InnerXml;
					this._content[name] = val;
				}
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement val = null;
				XmlAttribute valName = null;
				ICollection keys = this._content.Keys;
				foreach(Object o in keys)
				{
					val = document.CreateElement("Value");
					valName = document.CreateAttribute("name");
					valName.Value = (string)o;
					val.InnerXml = (string)this._content[o];
					val.Attributes.Append(valName);
					element.AppendChild(val);
				}
			}
			
			public void AddContent(string key, string val)
			{
				if(this._content.ContainsKey(key))
				{
					throw new KeyExists();
				}
				else
				{
					this._content[key] = val;
				}
			}
			
			public void ModifyContent(string key, string val)
			{
				this._content[key] = val;
			}
			
			public void DeleteContent(string key)
			{
				this._content.Remove(key);
			}
			
			public string GetContent(string key)
			{
				if(this._content.ContainsKey(key))
				{
					return (string)this._content[key];
				}
				else
				{
					throw new ItemNotFound("No match for given key ("+key+")");
				}
			}
			
			public bool Exists(string key)
			{
				return this._content.ContainsKey(key);
			}
		}
	}
}