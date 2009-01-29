
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Chat : BasicContent
		{
		
			private string _originator;
			private string _destination;
		
			public string originator
			{
				get{ return this._originator; }
				set{ this._originator = value; }
			}
			
			public string destination
			{
				get{ return this._destination; }
				set{ this._destination = value; }
			}
			
			public Chat() : base()
			{
				this._type = MessageContent.Type.Chat;
			}
		
			public Chat(XmlElement content) : base(content)
			{
				this._type = MessageContent.Type.Chat;
				this.Initialize(content);
			}
			
			public Chat(BasicContent clone) : base(clone)
			{
				this._type = MessageContent.Type.Chat;
				this.Initialize(this._raw_content);
			}
			
			private void Initialize(XmlElement content)
			{
				XmlNodeList list = content.GetElementsByTagName("ChatMessage");
				XmlElement element = (XmlElement)list.Item(0);
				if(element.HasAttribute("originator"))
				{
					this._originator = element.GetAttribute("originator");
				}
				if(element.HasAttribute("destination"))
				{
					this._destination = element.GetAttribute("destination");
				}
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = document.CreateElement("ChatMessage");
				XmlAttribute attr;
				if(this._destination != null)
				{
					attr = document.CreateAttribute("destination");
					attr.Value = this._destination;
					elm.Attributes.Append(attr);
				}
				if(this._originator != null)
				{
					attr = document.CreateAttribute("originator");
					attr.Value = this._originator;
					elm.Attributes.Append(attr);
				}
				base.DumpContent(document, elm);
				element.AppendChild(elm);
			}
		}
	}
}