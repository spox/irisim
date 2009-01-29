
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Event : BasicContent
		{
		
			private string _etype;
			
			public string etype
			{
				get{ return this._etype; }
				set{ this._etype = value; }
			}
		
			public Event() : base()
			{
				this._type = MessageContent.Type.Event;
			}
		
			public Event(XmlElement content) : base(content)
			{
				this._type = MessageContent.Type.Event;
				this.Initialize(content);
			}
			
			public Event(BasicContent clone) : base(clone)
			{
				this._type = MessageContent.Type.Event;
				this.Initialize(this._raw_content);
			}
			
			private void Initialize(XmlElement content)
			{
				XmlNodeList list = content.GetElementsByTagName("Event");
				XmlElement element = (XmlElement)list.Item(0);
				if(element.HasAttribute("type"))
				{
					this._etype = element.GetAttribute("type");
				}
				else
				{
					this._etype = null;
				}
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = null;
				XmlAttribute attr = null;
				elm = document.CreateElement("Event");
				attr = document.CreateAttribute("type");
				attr.Value = this._etype;
				elm.Attributes.Append(attr);
				base.DumpContent(document, elm);
				element.AppendChild(elm);
			}
		}
	}
}