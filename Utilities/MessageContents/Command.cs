
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Command : BasicContent
		{
			
			private string _ctype;
			
			public string ctype
			{
				get{ return this._ctype; }
				set{ this._ctype = value; }
			}
			
			public Command() : base()
			{
				this._type = MessageContent.Type.Command;
			}
			
			public Command(XmlElement content) : base(content)
			{
				this._type = MessageContent.Type.Command;
				this.Initialize(content);
			}
			
			public Command(BasicContent clone) : base(clone)
			{
				this._type = MessageContent.Type.Command;
				this.Initialize(this._raw_content);
			}
			
			private void Initialize(XmlElement content)
			{
				XmlNodeList list = content.GetElementsByTagName("Command");
				XmlElement element = (XmlElement)list.Item(0);
				if(element.HasAttribute("type"))
				{
					this._ctype = element.GetAttribute("type");
				}
				else
				{
					this._ctype = null;
				}
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = document.CreateElement("Command");
				XmlAttribute attr = document.CreateAttribute("type");
				attr.Value = this._ctype;
				elm.Attributes.Append(attr);
				base.DumpContent(document, elm);
				element.AppendChild(elm);
			}
		}
	}
}