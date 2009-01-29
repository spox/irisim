
using System.Xml;

namespace IrisIM
{
	namespace Utilities
	{
		public class Information : BasicContent
		{
		
			public Information() : base()
			{
				this._type = MessageContent.Type.Information;
			}
		
			public Information(XmlElement content) : base(content)
			{
				this._type = MessageContent.Type.Information;
			}
			
			public Information(BasicContent clone) : base(clone)
			{
				this._type = MessageContent.Type.Information;
			}
			
			override public void DumpContent(XmlDocument document, XmlElement element)
			{
				XmlElement elm = null;
				elm = document.CreateElement("Information");
				base.DumpContent(document, elm);
				element.AppendChild(elm);
			}
		}
	}
}