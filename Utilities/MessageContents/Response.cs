
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Response : BasicContent
		{
		
			private string _rtype;
		
			public string rtype
			{
				get{ return this._rtype; }
				set{ this._rtype = value; }
			}
			
			public Response() : base()
			{
				this._type = MessageContent.Type.Response;
			}
			
			public Response(XmlElement content) : base(content)
			{
				this._type = MessageContent.Type.Response;
				this.Initialize(content);
			}
			
			public Response(BasicContent clone) : base(clone)
			{
				this._type = MessageContent.Type.Response;
				this.Initialize(this._raw_content);
			}
			
			private void Initialize(XmlElement content)
			{
				XmlNodeList list = content.GetElementsByTagName("Response");
				XmlElement element = (XmlElement)list.Item(0);
				if(element.HasAttribute("type"))
				{
					this._rtype = element.GetAttribute("type");
				}
				else
				{	
					this._rtype = null;
				}
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = document.CreateElement("Response");
				XmlAttribute attr = document.CreateAttribute("type");
				attr.Value = this._rtype;
				elm.Attributes.Append(attr);
				base.DumpContent(document, elm);
				element.AppendChild(elm);
			}
		}
	}
}