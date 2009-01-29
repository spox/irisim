
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Unknown : ComplexContent
		{
			
			private string _message_data;
			
			public string message_data
			{
				get{ return this._message_data; }
			}

			public Unknown() : base()
			{
				this._type = MessageContent.Type.Unknown;
			}
			
			public Unknown(string content) : base()
			{
				this._type = MessageContent.Type.Unknown;
				this._message_data = content;
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = document.CreateElement("Unknown");
				elm.InnerXml = this._message_data;
				element.AppendChild(elm);
			}
		}
	}
}